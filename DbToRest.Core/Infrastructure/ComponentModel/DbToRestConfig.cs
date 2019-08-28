namespace DbToRest.Core.Infrastructure.ComponentModel
{
    public partial class DbToRestConfig
    {
        /**/

        public string DbDataConnection { get; set; }

        public string DbDatabaseName { get; set; }



        public bool DisplayFullErrorStack { get; set; }

        public bool RedisEnabled { get; set; }
        public string RedisConnectionString { get; set; }
        public int? RedisDatabaseId { get; set; }
        public bool UseRedisToStoreDataProtectionKeys { get; set; }
        public bool UseRedisForCaching { get; set; }
        public bool UseRedisToStorePluginsInfo { get; set; }

        public string UserAgentStringsPath { get; set; }

        public string CrawlerOnlyUserAgentStringsPath { get; set; }

        public bool DisableSampleDataDuringInstallation { get; set; }

        public bool UseFastInstallationService { get; set; }

        public string PluginsIgnoredDuringInstallation { get; set; }

        public bool ClearPluginShadowDirectoryOnStartup { get; set; }

        public bool CopyLockedPluginAssembilesToSubdirectoriesOnStartup { get; set; }

        public bool UseUnsafeLoadAssembly { get; set; }

        public bool UsePluginsShadowCopy { get; set; }

        public bool UseRowNumberForPaging { get; set; }

        public bool UseSessionStateTempDataProvider { get; set; }
    }
}