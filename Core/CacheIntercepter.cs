using System.Reflection;
using Castle.DynamicProxy;
using SimpleProxyCache.Attributes;
using SimpleProxyCache.Interfaces;

namespace SimpleProxyCache
{
    public class CacheIntercepter : IAsyncInterceptor
    {
        private readonly ICacheMethodUtility _cacheMethodUtility;

        public CacheIntercepter(ICacheMethodUtility cacheMethodUtility)
        {
            _cacheMethodUtility = cacheMethodUtility;
        }
        
        public void InterceptSynchronous(IInvocation invocation)
        {
            if (invocation.Method.GetCustomAttribute<CacheResultAttribute>() != null)
            {
                if (_cacheMethodUtility.Get(invocation.Method, invocation.Arguments) == null)
                {
                    invocation.Proceed();
                    
                    _cacheMethodUtility.Set(invocation.Method, invocation.Arguments, invocation);

                }
            }
        }

        public void InterceptAsynchronous(IInvocation invocation)
        {
            throw new System.NotImplementedException();
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            throw new System.NotImplementedException();
        }
    }
}