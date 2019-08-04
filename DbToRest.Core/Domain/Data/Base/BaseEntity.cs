using System;

namespace DbToRest.Core.Domain.Data
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
    }
}