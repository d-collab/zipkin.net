namespace Zipkin.Codecs.Thrift.TypeSystem
{
	public struct TList
	{
		public TType ElementType;
		public int Count;

		public TList(TType elementType, int count)
		{
			this.ElementType = elementType;
			this.Count = count;
		}
	}
}