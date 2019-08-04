using System;

namespace DbToRest.Core.Domain.Data
{
    public abstract class TrackEntity : BaseEntity
    {
        public Guid CreatedBy { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid CreatedByLogId { get; set; }

        public Guid? ModifiedBy { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public Guid? ModifiedByLogId { get; set; }

        public bool Deleted { get; set; }
    }
}