namespace DbToRest.Core.Domain.Data
{
    public class DataSource : TrackEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Client { get; set; }

        public string ConnectionString { get; set; }
    }
}