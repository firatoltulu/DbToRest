using System;
using System.Collections.Generic;

namespace DbToRest.Core.Infrastructure.Caching
{
    public interface ICache
    {
        int DefaultExpireDuration { get; }

        T Get<T>(string key, Func<T> item, int expire);

        T Get<T>(string key, Func<T> item);

        T Get<T>(string key);

        T GetHash<T>(string key);

        void Delete(string key);

        void Set<T>(string key, T item, int expire);

        void Set<T>(string key, T item);

        void SetHash<T>(string key, T item);

        List<string> Keys();

        bool Exists(string key);

        IEnumerable<T> Get<T>(string[] key);
    }
}