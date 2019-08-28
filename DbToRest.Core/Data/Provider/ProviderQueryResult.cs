namespace DbToRest.Core.Data.Provider
{
    public class ProviderQueryResult<T>
    {
        public T Result { get; set; }

        public int Count { get; set; }
    }
}