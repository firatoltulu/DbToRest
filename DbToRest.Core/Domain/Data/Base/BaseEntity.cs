using System;

namespace DbToRest.Core.Domain.Data
{
    public abstract class BaseEntity
    {
        public string Id { get; set; } = string.Empty;
    }
}