using System;
using System.Reflection;

namespace SimpleProxyCache.Exceptions
{
    public class TypeNotInterfaceException : Exception
    {
        public TypeNotInterfaceException(MemberInfo type) : base($"Type: {type.Name} is not interface") { }
    }
}