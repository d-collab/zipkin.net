namespace Zipkin.Codecs.Thrift.TypeSystem
{
	public struct TMessage
	{
		public string name;
		public TMessageType type;
		public int seqID;

		public TMessage(string name, TMessageType type, int seqid)
		{
			this.name = name;
			this.type = type;
			this.seqID = seqid;
		}
	}
}