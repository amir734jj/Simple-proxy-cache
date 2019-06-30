using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SimpleProxyCache.Interfaces;

namespace SimpleProxyCache.Logic
{
    internal class SimpleMethodMemoryCache : ICacheMethodUtility
    {
        private ConcurrentDictionary<MethodInfo, ConcurrentDictionary<object[], object>> _cache;

        public SimpleMethodMemoryCache()
        {
            _cache = new ConcurrentDictionary<MethodInfo, ConcurrentDictionary<object[], object>>();
        }

        /// <summary>
        ///     Set cache value
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="arguments"></param>
        /// <param name="result"></param>
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

        /// <summary>
        ///     Try get value given MethodInfo and result
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <param name="arguments"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public bool TryGet(MethodInfo methodInfo, object[] arguments, out object result)
        {
            if (_cache.ContainsKey(methodInfo))
            {
                var table = _cache[methodInfo];
                var keyValuePair = table.FirstOrDefault(x => x.Key.SequenceEqual(arguments));

                if (!keyValuePair.Equals( default(KeyValuePair<object[], object>)))
                {
                    result = keyValuePair.Value;

                    return true;
                }
            }

            result = null;
            
            return false;
        }
        
        /// <summary>
        ///     Invalidate cache
        /// </summary>
        public void Invalidate()
        {
            _cache = new ConcurrentDictionary<MethodInfo, ConcurrentDictionary<object[], object>>();
        }
    }
}