using System.Reflection;

namespace SimpleProxyCache.Interfaces
{
    public interface ICacheMethodUtility
    {
        object Get(MethodInfo methodInfo, object[] arguments);
        
        void Set(MethodInfo methodInfo, object[] arguments, object result);

        void Invalidate();
    }
}