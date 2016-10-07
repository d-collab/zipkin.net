namespace Zipkin.Codecs.Thrift.TypeSystem
{
	public struct TMap
	{
		public TType KeyType;
		public TType ValueType;
		public int Count;

		public TMap(TType keyType, TType valueType, int count)
		{
			this.KeyType = keyType;
			this.ValueType = valueType;
			this.Count = count;
		}
	}
}