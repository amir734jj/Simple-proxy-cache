using System;
using System.Reflection;

namespace SimpleProxyCache.Extensions
{
    internal static class MethodInfoExtension
    {
        /// <summary>
        ///     Test whether MethodInfo has custom attribute or not
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static bool HasCustomAttribute<T>(this MethodInfo methodInfo) where T : Attribute
        {
            return methodInfo.GetCustomAttribute<T>() != null;
        } 
    }
}