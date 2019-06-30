using System.Reflection;
using SimpleProxyCache.Attributes;
using SimpleProxyCache.Enums;
using SimpleProxyCache.Extensions;
using SimpleProxyCache.Interfaces;

namespace SimpleProxyCache.Logic
{
    internal class DefaultInvocationTypeResolver : IInvocationTypeResolver
    {
        public InvocationTypeEnum Resolve(MethodInfo method)
        {
            switch (method)
            {
                // Cache result
                case MethodInfo _ when method.HasCustomAttribute<CacheResultAttribute>():
                    return InvocationTypeEnum.Cache;
                // Invalidate the cache
                case MethodInfo _ when method.HasCustomAttribute<InvalidateCacheAttribute>():
                    return InvocationTypeEnum.Invalidate;
                default:
                    return InvocationTypeEnum.None;
            }
        }
    }
}