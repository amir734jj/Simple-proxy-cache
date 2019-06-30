using System.Reflection;
using SimpleProxyCache.Enums;

namespace SimpleProxyCache.Interfaces
{
    public interface IInvocationTypeResolver
    {
        InvocationTypeEnum Resolve(MethodInfo method);
    }
}