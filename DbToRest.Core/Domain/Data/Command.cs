using System;

namespace DbToRest.Core.Domain.Data
{
    public class Command
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public CommandType CommandType { get; set; }

        public string CommandText { get; set; }

        public Guid DataSourceId { get; set; }

        public Guid ProjectId { get; set; }
    }
}