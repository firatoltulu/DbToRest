using System.Runtime.Serialization;

namespace DbToRest.Core.Data
{
    /// <summary>
    /// Represents data provider type enumeration
    /// </summary>
    public enum DataProviderType
    {
        /// <summary>
        /// Unknown
        /// </summary>
        [EnumMember(Value = "")]
        Unknown,

        /// <summary>
        /// MS SQL Server
        /// </summary>
        [EnumMember(Value = "sqlserver")]
        SqlServer,

        /// <summary>
        /// MS SQL Server
        /// </summary>
        [EnumMember(Value = "couchdb")]
        CoubhDb
    }
}