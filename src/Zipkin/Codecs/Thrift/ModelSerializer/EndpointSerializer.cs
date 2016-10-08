namespace Zipkin.Codecs.Thrift.ModelSerializer
{
	using System;
	using System.Net;
	using Model;
	using Thrift;
	using TypeSystem;


	public static class EndpointSerializer
	{
		public static Endpoint Read(ThriftProtocol iprot)
		{
			var endpoint = new Endpoint();

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
							if (field.Type == TType.I32)
							{
								endpoint.IPAddress = new IPAddress((long) iprot.ReadI32());
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 2:
							if (field.Type == TType.I16)
							{
								endpoint.Port = (ushort) iprot.ReadI16();
							}
							else
							{
								iprot.Skip(field.Type);
							}
							break;
						case 3:
							if (field.Type == TType.String)
							{
								endpoint.ServiceName = iprot.ReadString();
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

			return endpoint;
		}

		public static void Write(Endpoint endpoint, ThriftProtocol oprot)
		{
			oprot.IncrementRecursionDepth();
			try
			{
				TStruct struc = new TStruct(/*"Endpoint"*/);
				oprot.WriteStructBegin(struc);
				TField field = new TField();
				if (endpoint.IPAddress != null)
				{
//					field.Name = "ipv4";
					field.Type = TType.I32;
					field.ID = 1;
					oprot.WriteFieldBegin(field);
					// oprot.WriteI32(endpoint.IPAddress.);

					var address = endpoint.IPAddress.GetAddressBytes();
					oprot.WriteI32(BitConverter.ToInt32(address, 0));

					oprot.WriteFieldEnd();
				}
				if (endpoint.Port.HasValue)
				{
//					field.Name = "port";
					field.Type = TType.I16;
					field.ID = 2;
					oprot.WriteFieldBegin(field);
					oprot.WriteI16((short) endpoint.Port.Value);
					oprot.WriteFieldEnd();
				}
				if (endpoint.ServiceName != null)
				{
//					field.Name = "service_name";
					field.Type = TType.String;
					field.ID = 3;
					oprot.WriteFieldBegin(field);
					oprot.WriteString(endpoint.ServiceName);
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