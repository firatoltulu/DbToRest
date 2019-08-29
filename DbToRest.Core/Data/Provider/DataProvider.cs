using DbToRest.Core.Infrastructure;
using DbToRest.Core.Infrastructure.ComponentModel;
using DbToRest.Core.Infrastructure.SmartForm;
using Fasterflect;
using MyCouch;
using Newtonsoft.Json.Linq;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.Client.Documents.Operations.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DbToRest.Core.Data.Provider
{
    public class DataProvider : IDataProvider
    {
        private readonly DbToRestConfig _dbToRestConfig = null;
        private readonly Lazy<IDocumentStore> store;
        private readonly ITypeFinder _typeFinder;
        private readonly ILogService _logService;
        private readonly string[] SystemTableNames = { };
        public IDocumentStore Database => store.Value;


        public DataProvider(
            DbToRestConfig dbToRestConfig,
            ITypeFinder typeFinder,
            ILogService logService
            )
        {
            _dbToRestConfig = dbToRestConfig;
            _typeFinder = typeFinder;
            _logService = logService;
            store = new Lazy<IDocumentStore>(CreateClient);
        }

        private IDocumentStore CreateClient()
        {
            var store = new DocumentStore
            {
                Urls = new string[] { _dbToRestConfig.DbDataConnection },
                Database = _dbToRestConfig.DbDatabaseName,
                Conventions =
                {
                    MaxNumberOfRequestsPerSession = 10,
                    UseOptimisticConcurrency = true
                }
            };

            store.Initialize();

            Initialize();

            return store;
        }

        private void Initialize()
        {
            try
            {
                setupIndices();
            }
            catch (System.Exception ex)
            {
                _logService.Error(ex.ToString());
            }
        }

        public dynamic Query(string query)
        {
            var command = SmartFilterCommand.Parse(query);
            var luceneQuery = SmartFilterRavenFormatter.Parse(command);
            return Query(command.From, luceneQuery, command.SortDescriptors, command.SelectDescriptors, command.Skip, command.Top);
        }

        public dynamic Query(string tableName, string query, string orderBy, string selectBy, int skip, int top)
        {
            var _orderBy = SmartFilterDescriptorSerializer.Deserialize<SortDescriptor>(orderBy);
            var _selectBy = SmartFilterDescriptorSerializer.Deserialize<SelectDescriptor>(orderBy);

            return Query(tableName, query, _orderBy, _selectBy, skip, top);
        }

        public dynamic Query(string tableName, string query, IList<SortDescriptor> orderBy, IList<SelectDescriptor> selectBy, int skip, int top)
        {
            Expando result = new Expando();

            int totalCount = 0;
            List<dynamic> totalResult = null;

            StringBuilder ravenQueryBuilder = new StringBuilder();

            using (var session = Database.OpenSession())
            {
                string ravenTableName = tableName;
                IndexDefinition indexName = null;

                if (SystemTableNames.Contains(tableName) == false)
                    indexName = session.Advanced.DocumentStore.Maintenance.Send(new GetIndexOperation(ravenTableName));

                var fromName = indexName != null ? string.Format("index '{0}'", indexName.Name) : ravenTableName;

                ravenQueryBuilder.Append(" from ");
                ravenQueryBuilder.Append(fromName);
                ravenQueryBuilder.Append(" where ");

                if (query.IsNullOrEmpty() == false)
                {
                    ravenQueryBuilder.Append(query);
                    ravenQueryBuilder.Append(" and Deleted = false ");
                }
                else
                {
                    ravenQueryBuilder.Append(" Deleted = false ");
                }

                if (orderBy.Count > 0)
                {
                    ravenQueryBuilder.Append(" order by ");
                    ravenQueryBuilder.Append(string.Join(",", orderBy.Select(f => f.SerializeNoScore())));
                }

                var rawResult = session.Advanced.RawQuery<dynamic>(ravenQueryBuilder.ToString());
                rawResult.AfterQueryExecuted(act =>
                {
                    totalCount = act.TotalResults;
                });
                totalResult = rawResult.Skip(skip).Take(top).ToList();
            }

            result.Properties.Add("Count", totalCount);
            result.Properties.Add("Result", totalResult);

            return result;
        }

        public dynamic Query(SmartFilterCommand command)
        {
            var luceneQuery = SmartFilterRavenFormatter.Parse(command);
            return Query(command.From, luceneQuery, command.SortDescriptors, command.SelectDescriptors, command.Skip, command.Top);
        }

        public ProviderQueryResult<List<T>> Query<T>(string tableName, string query, string orderBy, string selectBy, int skip, int top)
        {
            var _orderBy = SmartFilterDescriptorSerializer.Deserialize<SortDescriptor>(orderBy);
            var _selectBy = SmartFilterDescriptorSerializer.Deserialize<SelectDescriptor>(orderBy);
            return Query<T>(tableName, query, _orderBy, _selectBy, skip, top);
        }

        public ProviderQueryResult<List<T>> Query<T>(SmartFilterCommand command)
        {
            var luceneQuery = SmartFilterRavenFormatter.Parse(command);
            return Query<T>(command.From, luceneQuery, command.SortDescriptors, command.SelectDescriptors, command.Skip, command.Top);
        }

        public ProviderQueryResult<List<T>> Query<T>(string tableName, string query, IList<SortDescriptor> orderBy, IList<SelectDescriptor> selectBy, int skip, int top)
        {
            ProviderQueryResult<List<T>> resultQuery = new ProviderQueryResult<List<T>>();
            var result = Query(tableName, query, orderBy, selectBy, skip, top);
            resultQuery.Result = (result.Result as List<dynamic>).Select(f =>
            {
                if (f is JObject)
                    return ((JObject)f).ToObject<T>();
                else
                    return (T)f;
            }).ToList();
            resultQuery.Count = Convert.ToInt32(result.Count);
            return resultQuery;
        }

        public ProviderQueryResult<List<T>> Query<T>(string query)
        {
            var command = SmartFilterCommand.Parse(query);
            var luceneQuery = SmartFilterRavenFormatter.Parse(command);
            return Query<T>(command.From, luceneQuery, command.SortDescriptors, command.SelectDescriptors, command.Skip, command.Top);
        }

        public void SetupIndexes()
        {
            setupIndices();
        }



        private void setupIndices()
        {
            var indexCreations = _typeFinder.FindClassesOfType<AbstractIndexCreationTask>().ToList();
            indexCreations.RemoveAll(t => t.Namespace.StartsWith("DbToRest.Core.Domain.Data.Indexing") == false);
            indexCreations.Each(t =>
            {
                Database.ExecuteIndex((AbstractIndexCreationTask)t.CreateInstance());
            });
        }

        private void setupTransform()
        {

        }
    }
}