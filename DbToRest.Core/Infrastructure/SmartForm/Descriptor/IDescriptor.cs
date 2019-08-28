namespace DbToRest.Core.Infrastructure.SmartForm
{
    public interface IDescriptor
    {
        void Deserialize(string source);
        string Serialize();
    }
}
