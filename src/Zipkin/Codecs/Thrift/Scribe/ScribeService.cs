namespace Zipkin.Codecs.Thrift.Scribe
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	using TypeSystem;


	public class ScribeService
	{
		private readonly ThriftProtocol _protocol;
		private int _seqid;

		public ScribeService(ThriftProtocol protocol)
		{
			_protocol = protocol;
		}

		public Task<ResultCode> Log(LogEntry message)
		{
			return this.Log(new [] { message });
		}

		public Task<ResultCode> Log(IList<LogEntry> messages)
		{
			_protocol.WriteMessageBegin(new TMessage("Log", TMessageType.Call, _seqid++));
			var args = new LogOperationArgs()
			{
				Messages = messages
			};
			args.Write(_protocol);
			_protocol.WriteMessageEnd();
			_protocol.Flush();

			return Task.FromResult(ReceiveResultFromLogOperation());
		}

		public ResultCode ReceiveResultFromLogOperation()
		{
			TMessage msg = _protocol.ReadMessageBegin();
			if (msg.type == TMessageType.Exception)
			{
				var ex = TApplicationException.Read(_protocol);
				_protocol.ReadMessageEnd();
				throw ex;
			}
			var result = new LogOperationResult();
			result.Read(_protocol);
			_protocol.ReadMessageEnd();
			if (result.Success.HasValue)
			{
				return result.Success.Value;
			}
			throw new TApplicationException(TApplicationException.ExceptionType.MissingResult, "Log failed: unknown result");
		}

		public class LogOperationArgs
		{
			public IList<LogEntry> Messages { get; set; }

			public void Read(ThriftProtocol iprot)
			{
				iprot.IncrementRecursionDepth();
				try
				{
					TField field;
					iprot.ReadStructBegin();
					while (true)
					{
						field = iprot.ReadFieldBegin();
						if (field.Type == TType.Stop)
						{
							break;
						}
						switch (field.ID)
						{
							case 1:
								if (field.Type == TType.List)
								{
									{
										Messages = new List<LogEntry>();
										TList list = iprot.ReadListBegin();
										for (int i = 0; i < list.Count; ++i)
										{
											var entry = LogEntrySerializer.Read(iprot);
											Messages.Add(entry);
										}
										iprot.ReadListEnd();
									}
								}
								else
								{
									iprot.Skip(field.Type);
								}
								break;
							default:
								iprot.Skip(field.Type);
								break;
						}
						iprot.ReadFieldEnd();
					}
					iprot.ReadStructEnd();
				}
				finally
				{
					iprot.DecrementRecursionDepth();
				}
			}

			public void Write(ThriftProtocol oprot)
			{
				oprot.IncrementRecursionDepth();
				try
				{
					var struc = new TStruct(/*"Log_args"*/);
					oprot.WriteStructBegin(struc);
					TField field = new TField();
					if (Messages != null)
					{
//						field.Name = "messages";
						field.Type = TType.List;
						field.ID = 1;
						oprot.WriteFieldBegin(field);
						{
							oprot.WriteListBegin(new TList(TType.Struct, Messages.Count));
							foreach (LogEntry logEntry in Messages)
							{
								// logEntry.Write(oprot);
								LogEntrySerializer.Write(oprot, logEntry);
							}
							oprot.WriteListEnd();
						}
						oprot.WriteFieldEnd();
					}
					oprot.WriteFieldStop();
					oprot.WriteStructEnd();
				}
				finally
				{
					oprot.DecrementRecursionDepth();
				}
			}
		}


		public class LogOperationResult
		{
			public ResultCode? Success { get; set; }

			public void Read(ThriftProtocol iprot)
			{
				iprot.IncrementRecursionDepth();
				try
				{
					TField field;
					iprot.ReadStructBegin();
					while (true)
					{
						field = iprot.ReadFieldBegin();
						if (field.Type == TType.Stop)
						{
							break;
						}
						switch (field.ID)
						{
							case 0:
								if (field.Type == TType.I32)
								{
									Success = (ResultCode)iprot.ReadI32();
								}
								else
								{
									iprot.Skip(field.Type);
								}
								break;
							default:
								iprot.Skip(field.Type);
								break;
						}
						iprot.ReadFieldEnd();
					}
					iprot.ReadStructEnd();
				}
				finally
				{
					iprot.DecrementRecursionDepth();
				}
			}

			public void Write(ThriftProtocol oprot)
			{
				oprot.IncrementRecursionDepth();
				try
				{
					TStruct struc = new TStruct(/*"Log_result"*/);
					oprot.WriteStructBegin(struc);
					TField field = new TField();

					if (this.Success != null)
					{
//						field.Name = "Success";
						field.Type = TType.I32;
						field.ID = 0;
						oprot.WriteFieldBegin(field);
						oprot.WriteI32((int)Success.Value);
						oprot.WriteFieldEnd();
					}
					oprot.WriteFieldStop();
					oprot.WriteStructEnd();
				}
				finally
				{
					oprot.DecrementRecursionDepth();
				}
			}
		}
	}
}