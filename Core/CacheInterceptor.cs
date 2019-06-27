using System.Threading.Tasks;
using Castle.DynamicProxy;
using SimpleProxyCache.Attributes;
using SimpleProxyCache.Extensions;
using SimpleProxyCache.Interfaces;

namespace SimpleProxyCache
{
    public class CacheInterceptor : IAsyncInterceptor
    {
        private readonly ICacheMethodUtility _cacheMethodUtility;

        public CacheInterceptor(ICacheMethodUtility cacheMethodUtility)
        {
            _cacheMethodUtility = cacheMethodUtility;
        }
        
        /// <summary>
        ///     Sync interceptor
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptSynchronous(IInvocation invocation)
        {
            var (method, arguments) = (invocation.Method, invocation.Arguments);

            // Cache result
            if (method.HasCustomAttribute<CacheResultAttribute>())
            {
                if (_cacheMethodUtility.TryGet(method, arguments, out var result))
                {
                    invocation.ReturnValue = result;
                }
                else
                {
                    invocation.Proceed();

                    _cacheMethodUtility.Set(method, arguments, invocation.ReturnValue);
                }
            }
            // Invalidate the cache
            else if (method.HasCustomAttribute<InvalidateCacheAttribute>())
            {
                _cacheMethodUtility.Invalidate();

                invocation.Proceed();
            }
        }

        /// <summary>
        ///     No caching here. Can only bust the cache
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptAsynchronous(IInvocation invocation)
        {
            var method = invocation.Method;

            if (method.HasCustomAttribute<InvalidateCacheAttribute>())
            {
                _cacheMethodUtility.Invalidate();
            }
            
            invocation.Proceed();
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var (method, arguments) = (invocation.Method, invocation.Arguments);

            // Cache result
            if (method.HasCustomAttribute<CacheResultAttribute>())
            {
                if (_cacheMethodUtility.TryGet(method, arguments, out var result))
                {
                    invocation.ReturnValue = Task.FromResult((TResult)result);
                }
                else
                {
                    invocation.Proceed();

                    var taskResult = ((Task<TResult>) invocation.ReturnValue).Result;

                    _cacheMethodUtility.Set(method, arguments, taskResult);
                    
                    invocation.ReturnValue = Task.FromResult(taskResult);
                }
            }
            // Invalidate the cache
            else if (method.HasCustomAttribute<InvalidateCacheAttribute>())
            {
                _cacheMethodUtility.Invalidate();

                invocation.Proceed();
            }
        }
    }
}