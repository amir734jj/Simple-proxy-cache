using System;
using Castle.DynamicProxy;
using SimpleProxyCache.Exceptions;
using SimpleProxyCache.Interfaces;

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

        public CacheInterceptorBuilderFinalize<T> WithStore(ICacheMethodUtility cacheMethodUtility)
        {
            return new CacheInterceptorBuilderFinalize<T>(_proxyGenerator, cacheMethodUtility);
        }
    }

    public class CacheInterceptorBuilderFinalize<T> where T : class
    {
        private readonly ProxyGenerator _proxyGenerator;
        private readonly ICacheMethodUtility _cacheMethodUtility;

        public CacheInterceptorBuilderFinalize(ProxyGenerator proxyGenerator,
            ICacheMethodUtility cacheMethodUtility)
        {
            _proxyGenerator = proxyGenerator;
            _cacheMethodUtility = cacheMethodUtility;
        }

        public T Build<TS>(TS instance) where TS : T
        {
            return _proxyGenerator.CreateInterfaceProxyWithTargetInterface<T>
            (
                instance,
                new CacheInterceptor(_cacheMethodUtility)
            );
        }
    }
}