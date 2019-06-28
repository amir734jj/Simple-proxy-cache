using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using SimpleProxyCache.Enums;
using SimpleProxyCache.Interfaces;
using SimpleProxyCache.Logic;

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

            switch (DetermineInvocationType.Resolve(method))
            {
                case InvocationTypeEnum.Cache:
                    if (_cacheMethodUtility.TryGet(method, arguments, out var result))
                    {
                        invocation.ReturnValue = result;
                    }
                    else
                    {
                        invocation.Proceed();
                        _cacheMethodUtility.Set(method, arguments, invocation.ReturnValue);
                    }
                    break;
                case InvocationTypeEnum.Invalidate:
                    _cacheMethodUtility.Invalidate();
                    invocation.Proceed();
                    break;
                case InvocationTypeEnum.None:
                    invocation.Proceed();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     No caching here. Can only bust the cache
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptAsynchronous(IInvocation invocation)
        {
            var method = invocation.Method;

            switch (DetermineInvocationType.Resolve(method))
            {
                case InvocationTypeEnum.Invalidate:
                    _cacheMethodUtility.Invalidate();
                    break;
            }
            
            invocation.Proceed();
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var (method, arguments) = (invocation.Method, invocation.Arguments);

            switch (DetermineInvocationType.Resolve(method))
            {
                case InvocationTypeEnum.Cache:
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
                    break;
                case InvocationTypeEnum.Invalidate:
                    _cacheMethodUtility.Invalidate();

                    invocation.Proceed();
                    break;
                case InvocationTypeEnum.None:
                    invocation.Proceed();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}