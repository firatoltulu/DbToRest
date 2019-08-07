using DbToRest.Core.Infrastructure.ComponentModel;
using MyCouch;
using System;

namespace DbToRest.Core.Data.Provider
{
    public class DataProvider : IDataProvider
    {
        private readonly DbToRestConfig _dbToRestConfig = null;
        private Lazy<IMyCouchClient> store;

        public DataProvider(DbToRestConfig dbToRestConfig)
        {
            _dbToRestConfig = dbToRestConfig;
            store = new Lazy<IMyCouchClient>(CreateClient);
        }

        private IMyCouchClient CreateClient()
        {
            return new MyCouch.MyCouchClient(_dbToRestConfig.CouchDbDataConnection, _dbToRestConfig.CouchDbDatabaseName);
        }

        public IMyCouchClient Provider => store.Value;
    }
}