using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleProxyCache.Interfaces;

namespace SimpleProxyCache.Logic
{
    public class SimpleMethodMemoryCache : ICacheMethodUtility
    {
        private ConcurrentDictionary<MethodInfo, ConcurrentDictionary<object[], object>> _cache;

        public SimpleMethodMemoryCache()
        {
            _cache = new ConcurrentDictionary<MethodInfo, ConcurrentDictionary<object[], object>>();
        }

        public object Get(MethodInfo methodInfo, object[] arguments)
        {
            if (_cache.ContainsKey(methodInfo))
            {
                var table = _cache[methodInfo];
                var keyValuePair = table.FirstOrDefault(x => x.Key.SequenceEqual(arguments));
                
                return keyValuePair.Equals( default(KeyValuePair<object[], object>)) ? keyValuePair.Value : null;
            }

            return null;
        }

        public void Set(MethodInfo methodInfo, object[] arguments, object result)
        {
            ConcurrentDictionary<object[], object> table;
            
            if (_cache.ContainsKey(methodInfo))
            {
                table = _cache[methodInfo];
                table[arguments] = result;
            }
            else
            {
                table = new ConcurrentDictionary<object[], object> {[arguments] = result};
                _cache[methodInfo] = table;
            }
        }

        public void Invalidate()
        {
            _cache = new ConcurrentDictionary<MethodInfo, ConcurrentDictionary<object[], object>>();
        }
    }
}