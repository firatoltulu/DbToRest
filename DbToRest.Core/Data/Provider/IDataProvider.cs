using DbToRest.Core.Infrastructure.SmartForm;
using MyCouch;
using Raven.Client.Documents;
using System;
using System.Collections.Generic;
using System.Text;

namespace DbToRest.Core.Data.Provider
{
    public interface IDataProvider
    {
        IDocumentStore Database { get; }

        dynamic Query(string query);

        dynamic Query(SmartFilterCommand command);

        dynamic Query(string tableName, string query, string orderBy, string selectBy, int skip, int top);

        dynamic Query(string tableName, string query, IList<SortDescriptor> orderBy, IList<SelectDescriptor> selectBy, int skip, int top);

        ProviderQueryResult<List<T>> Query<T>(SmartFilterCommand command);

        ProviderQueryResult<List<T>> Query<T>(string tableName, string query, string orderBy, string selectBy, int skip, int top);
        
        ProviderQueryResult<List<T>> Query<T>(string tableName, string query, IList<SortDescriptor> orderBy, IList<SelectDescriptor> selectBy, int skip, int top);

        ProviderQueryResult<List<T>> Query<T>(string query);

        void SetupIndexes();

    }
}
