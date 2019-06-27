using System;
using Castle.DynamicProxy;
using SimpleProxyCache.Exceptions;
using SimpleProxyCache.Interfaces;

namespace SimpleProxyCache.Builders
{
    public static class CacheInterceptorBuilder
    {
        public static CacheInterceptorBuilderProxyGenerator<T> New<T>()
        {
            var type = typeof(T);

            if (!type.IsInterface)
            {
                throw new TypeNotInterfaceException(type);
            }

            return new CacheInterceptorBuilderProxyGenerator<T>(type);
        }
    }

    public class CacheInterceptorBuilderProxyGenerator<T>
    {
        private readonly Type _type;

        public CacheInterceptorBuilderProxyGenerator(Type type)
        {
            _type = type;
        }

        public CacheInterceptorBuilderDal<T> WithProxyGenerator(ProxyGenerator proxyGenerator)
        {
            return new CacheInterceptorBuilderDal<T>(_type, proxyGenerator);
        }
    }

    public class CacheInterceptorBuilderDal<T>
    {
        private readonly Type _type;
        private readonly ProxyGenerator _proxyGenerator;

        public CacheInterceptorBuilderDal(Type type, ProxyGenerator proxyGenerator)
        {
            _type = type;
            _proxyGenerator = proxyGenerator;
        }

        public CacheInterceptorBuilderFinalize<T> WithStore(ICacheMethodUtility cacheMethodUtility)
        {
            return new CacheInterceptorBuilderFinalize<T>(_type, _proxyGenerator, cacheMethodUtility);
        }
    }

    public class CacheInterceptorBuilderFinalize<T>
    {
        private readonly Type _type;
        private readonly ProxyGenerator _proxyGenerator;
        private readonly ICacheMethodUtility _cacheMethodUtility;

        public CacheInterceptorBuilderFinalize(Type type, ProxyGenerator proxyGenerator,
            ICacheMethodUtility cacheMethodUtility)
        {
            _type = type;
            _proxyGenerator = proxyGenerator;
            _cacheMethodUtility = cacheMethodUtility;
        }

        public T Build<TS>(TS instance) where TS : T
        {
            return (T) _proxyGenerator.CreateInterfaceProxyWithTargetInterface
            (_type,
                instance,
                new CacheInterceptor(_cacheMethodUtility)
            );
        }
    }
}