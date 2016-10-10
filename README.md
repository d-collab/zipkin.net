# Zipkin.net

A .net client for [zipkin](http://zipkin.io/) - a great tool for distributed tracing. This client is focused on low mem footprint / allocations.


### Quick intro

```C#
using (new StartClientTrace("client-op")) // Starts a root trace span
{
	var crossProcessBag = new Dictionary<string,object>();
	TraceContextPropagation.PropagateTraceIdOnto(crossProcessBag);

	Thread.Sleep(20);

	using (new StartServerTrace("server-op", crossProcessBag).SetLocalComponentName("fake-server")) // new span
	{
		using (new LocalTrace("op1").AnnotateWith(PredefinedTag.SqlQuery, "select * from  ...")) // local span + annotation
		{
			Thread.Sleep(70);
		}

		using (var trace = new LocalTrace("op2"))
		{
			Thread.Sleep(90);

			trace.AnnotateWith(PredefinedTag.Error, "error message"); // binary annotation
		}

		using (new LocalTrace("op2").TimeAnnotateWith(PredefinedTag.ServerSend)) // timed annotation
		{
			Thread.Sleep(90);
		}
	}
}
```

The code above translates into:

![alt text](http://i.imgur.com/cfNn4Q2.png "Capture of zipkin")


### How to set it up

Just use the ZipkinBootstrapper:

```C#
new Zipkin.ZipkinBootstrapper("my-service")
			.ZipkinAt("localhost") // where's the zipkin server?
			.WithSampleRate(0.1)   // 0.1 means trace 10% of calls
			.Start();
```

In addition, you may also call:

* ```WithMetrics(RecorderMetrics)```: to provide a custom RecorderMetrics instance 
* ```WithCodec(Codec)```: to replace Thrift for Json if you want
* ```DispatchTo(SpanDispatcher)```: to use a different dispatcher. By default we use the rest endpoints


### How to use it

Zipkin's role is to receive ```Span``` information from different services, and assemble them for a nice display, while making them queryable. 
To be considered the same trace, they need to share a Trace id. To have a proper hierarchical view, they need to use ParentId properly.

Therefore this is extremely useful for distributed system, but in order for it to work, your system needs to carry some information across service calls. 

For example, 
- if you use http to make Rpc calls, you'd like use extra http headers
- if you use amqp messages, add some extra headers
- If you use thrift or 0MQ, you'd probably have to change the messages since they dont expose envelopes

In our tracing api it all starts with a ```StartClientTrace```, albeit "client side" is purposely loosely defined concept. 

#### Tracing

For all tracing types, the Span is submitted upon disposal of the instance, so it plays well with the C#'s ```using()``` idiom. 

##### StartClientTrace

Starts a client-send trace scope, contingent on the sampling rate. This would the root trace.

```C#
using (new StartClientTrace("client-op"))
{
}
```

##### StartServerTrace

Continues a trace on the server side, if there was one. So it tries to get the trace information from the dictionary (representing http headers or something to that effect) and sets up a trace context accordingly. If nothing's found, it would have no effect. 

```C#
using (new StartServerTrace("server-op", crossProcessDictionary))
{
}
```

##### LocalTrace

Should be used when there's already a trace set up on the context. Use it to capture some relevant activity you would like to see displayed. They can be nested. 

```C#
using (new LocalTrace("query-accounts"))
{
}
```

##### Adding info about points in time

This is done with ```Annotation```  on the ```Span```, which contains a Timestamp and a kind. 

The semantically known kinds can be found at StandardAnnotationKeys.cs, while other common types can be found at CustomAnnotationKeys. 

You're free to define your own.


##### Adding information

This is done with ```BinaryAnnotation``` on the ```Span```. It differs from ```Annotation``` in not including a Timestamp but allows you to add extra info. 

Use it to capture sql, http information, etc.


#### Crossing process boundaries

Use the [TraceContextPropagation](https://github.com/clearctvm/zipkin.net/blob/master/src/Zipkin/TraceContextPropagation.cs) to propagate and restore tracing context information. 






