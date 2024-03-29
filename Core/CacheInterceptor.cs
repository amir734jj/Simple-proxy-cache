﻿using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using SimpleProxyCache.Enums;
using SimpleProxyCache.Interfaces;
using SimpleProxyCache.Logic;

namespace SimpleProxyCache
{
    internal class CacheInterceptor : IAsyncInterceptor
    {
        private readonly ICacheMethodUtility _cacheMethodUtility;
        private readonly IInvocationTypeResolver _invocationTypeResolver;

        public CacheInterceptor(IInvocationTypeResolver invocationTypeResolver, ICacheMethodUtility cacheMethodUtility)
        {
            _invocationTypeResolver = invocationTypeResolver;
            _cacheMethodUtility = cacheMethodUtility;
        }
        
        /// <inheritdoc />
        /// <summary>
        ///     Sync interceptor
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptSynchronous(IInvocation invocation)
        {
            var (method, arguments) = (invocation.Method, invocation.Arguments);

            switch (_invocationTypeResolver.Resolve(method))
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

        /// <inheritdoc />
        /// <summary>
        ///     No caching here. Can only bust the cache
        /// </summary>
        /// <param name="invocation"></param>
        public void InterceptAsynchronous(IInvocation invocation)
        {
            var method = invocation.Method;

            switch (_invocationTypeResolver.Resolve(method))
            {
                case InvocationTypeEnum.Invalidate:
                    _cacheMethodUtility.Invalidate();
                    break;
                case InvocationTypeEnum.Cache:
                    break;
                case InvocationTypeEnum.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            invocation.Proceed();
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var (method, arguments) = (invocation.Method, invocation.Arguments);

            switch (_invocationTypeResolver.Resolve(method))
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