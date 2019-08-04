namespace DbToRest.Core.Domain.Data
{
    public class Project : TrackEntity
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public string Domain { get; set; }

        public string ModuleFile { get; set; }
    }
}