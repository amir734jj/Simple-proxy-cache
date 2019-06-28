using System.Reflection;
using SimpleProxyCache.Attributes;
using SimpleProxyCache.Enums;
using SimpleProxyCache.Extensions;

namespace SimpleProxyCache.Logic
{
    public static class DetermineInvocationType
    {
        public static InvocationTypeEnum Resolve(MethodInfo method)
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