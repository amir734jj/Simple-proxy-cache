using System;
using System.Reflection;
using Castle.DynamicProxy;
using SimpleProxyCache.Enums;
using SimpleProxyCache.Exceptions;
using SimpleProxyCache.Interfaces;
using SimpleProxyCache.Logic;

namespace SimpleProxyCache.Builders
{
    public static class CacheInterceptorBuilder
    {
        public static CacheInterceptorBuilderProxyGenerator<T> New<T>() where T : class
        {
            var type = typeof(T);

            if (!type.IsInterface)
            {
                throw new TypeNotInterfaceException(type);
            }

            return new CacheInterceptorBuilderProxyGenerator<T>();
        }
    }

    public class CacheInterceptorBuilderProxyGenerator<T> where T : class
    {
        public CacheInterceptorBuilderDal<T> WithProxyGenerator(ProxyGenerator proxyGenerator)
        {
            return new CacheInterceptorBuilderDal<T>(proxyGenerator);
        }
    }

    public class CacheInterceptorBuilderDal<T> where T : class
    {
        private readonly ProxyGenerator _proxyGenerator;

        public CacheInterceptorBuilderDal(ProxyGenerator proxyGenerator)
        {
            _proxyGenerator = proxyGenerator;
        }
        
        public CacheInterceptorBuilderInvocationTypeResolver<T> WithDefaultStore()
        {
            return new CacheInterceptorBuilderInvocationTypeResolver<T>(_proxyGenerator, new SimpleMethodMemoryCache());
        }

        public CacheInterceptorBuilderInvocationTypeResolver<T> WithStore(ICacheMethodUtility cacheMethodUtility)
        {
            return new CacheInterceptorBuilderInvocationTypeResolver<T>(_proxyGenerator, cacheMethodUtility);
        }
    }
    
    public class CacheInterceptorBuilderInvocationTypeResolver<T> where T : class
    {
        private readonly ProxyGenerator _proxyGenerator;
        private readonly ICacheMethodUtility _cacheMethodUtility;

        public CacheInterceptorBuilderInvocationTypeResolver(ProxyGenerator proxyGenerator,
            ICacheMethodUtility cacheMethodUtility)
        {
            _proxyGenerator = proxyGenerator;
            _cacheMethodUtility = cacheMethodUtility;
        }
        
        public CacheInterceptorBuilderFinalize<T> WithDefaultInvocationTypeResolver()
        {
            return new CacheInterceptorBuilderFinalize<T>(_proxyGenerator, _cacheMethodUtility, new DefaultInvocationTypeResolver());
        }

        public CacheInterceptorBuilderFinalize<T> WithInvocationTypeResolver(IInvocationTypeResolver invocationTypeResolver)
        {
            return new CacheInterceptorBuilderFinalize<T>(_proxyGenerator, _cacheMethodUtility, invocationTypeResolver);
        }
        
        public CacheInterceptorBuilderFinalize<T> WithInvocationTypeResolver(Func<MethodInfo, InvocationTypeEnum> invocationTypeResolver)
        {
            return new CacheInterceptorBuilderFinalize<T>(_proxyGenerator, _cacheMethodUtility, new InvocationTypeResolverBuilder(invocationTypeResolver));
        }
    }

    public class CacheInterceptorBuilderFinalize<T> where T : class
    {
        private readonly ProxyGenerator _proxyGenerator;
        private readonly ICacheMethodUtility _cacheMethodUtility;
        private readonly IInvocationTypeResolver _invocationTypeResolver;

        public CacheInterceptorBuilderFinalize(ProxyGenerator proxyGenerator,
            ICacheMethodUtility cacheMethodUtility, IInvocationTypeResolver invocationTypeResolver)
        {
            _proxyGenerator = proxyGenerator;
            _cacheMethodUtility = cacheMethodUtility;
            _invocationTypeResolver = invocationTypeResolver;
        }

        public T Build<TS>(TS instance) where TS : T
        {
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>
            (
                instance,
                new CacheInterceptor(_invocationTypeResolver, _cacheMethodUtility)
            );
        }
    }
}