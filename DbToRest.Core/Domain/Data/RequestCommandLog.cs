using System;

namespace DbToRest.Core.Domain.Data
{
    public class RequestCommandLog : TrackEntity
    {
        public string CommandName { get; set; }

        public string Parameters { get; set; }

        public DateTime ResponseTime { get; set; }

        public DateTime RequestTime { get; set; }

        public string OutputContent { get; set; }
    }
}