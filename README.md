# Zipkin.net

A .net client for [zipkin](http://zipkin.io/) - a great tool for distributed tracing. 

### Quick intro

```C#
using (new StartClientTrace("client-op")) // Starts a root trace + span
{
	var crossProcessBag = new Dictionary<string,object>();
	TraceContextPropagation.PropagateTraceIdOnto(crossProcessBag);

	Thread.Sleep(20);

	using (new StartServerTrace("server-op", crossProcessBag).SetLocalComponentName("fake-server"))
	{
		using (new LocalTrace("op1").AnnotateWith(PredefinedTag.SqlQuery, "select * from  ..."))
		{
			Thread.Sleep(70);
		}

		using (var trace = new LocalTrace("op2"))
		{
			Thread.Sleep(90);

			trace.AnnotateWith(PredefinedTag.Error, "error message"); // mark it with an error
		}

		using (new LocalTrace("op2").AnnotateWithTag(PredefinedTag.ServerSend))
		{
			Thread.Sleep(90);
		}
	}
}
```

This code above would translate into:

![alt text](http://i.imgur.com/cfNn4Q2.png "Capture of zipkin")


### How to set it up

Just use the ZipkinBootstrapper:

```C#
new Zipkin.ZipkinBootstrapper("my-service")
			.ZipkinAt("localhost") // where's the zipkin server?
			.WithSampleRate(0.1)   // 0.1 means trace 10% 
			.Start();
```

In addition, you may config 

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



#### Crossing process boundaries

Use the [TraceContextPropagation](https://github.com/clearctvm/zipkin.net/blob/master/src/Zipkin/TraceContextPropagation.cs) to propagate and restore tracing context information. 






