using System;
using System.Reflection;
using SimpleProxyCache.Enums;
using SimpleProxyCache.Interfaces;

namespace SimpleProxyCache.Logic
{
    public class InvocationTypeResolverBuilder : IInvocationTypeResolver
    {
        private readonly Func<MethodInfo, InvocationTypeEnum> _lambda;

        public InvocationTypeResolverBuilder(Func<MethodInfo, InvocationTypeEnum> lambda)
        {
            _lambda = lambda;
        }
        
        public InvocationTypeEnum Resolve(MethodInfo method)
        {
            return _lambda(method);
        }
    }
}