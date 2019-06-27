using System.Reflection;

namespace SimpleProxyCache.Interfaces
{
    public interface ICacheMethodUtility
    {
        bool TryGet(MethodInfo methodInfo, object[] arguments, out object result);
        
        void Set(MethodInfo methodInfo, object[] arguments, object result);

        void Invalidate();
    }
}