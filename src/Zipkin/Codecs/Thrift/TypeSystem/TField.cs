namespace Zipkin.Codecs.Thrift.TypeSystem
{
	public enum TMessageType
	{
		Call = 1,
		Reply = 2,
		Exception = 3,
		Oneway = 4
	}

	public struct TField
	{
//		public string Name;
		public TType Type;
		public short ID;

		public TField(/*string name, */TType type, short id)
		{
//			this.Name = name;
			this.Type = type;
			this.ID = id;
		}
	}
}