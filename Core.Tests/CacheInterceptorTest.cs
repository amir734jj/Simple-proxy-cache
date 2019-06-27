using System;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Moq;
using SimpleProxyCache.Attributes;
using SimpleProxyCache.Builders;
using SimpleProxyCache.Logic;
using Xunit;

namespace Core.Tests
{
    public interface IFoo
    {
        [CacheResult]
        string Handle(string id);

        [CacheResult]
        Task<string> HandleAsync(string id);
    }
    
    public class CacheInterceptorTest
    {
        [Fact]
        public void Test1()
        {
            var proxyGenerator = new ProxyGenerator();

            var called = false;
            
            var fooMock = new Mock<IFoo>();
            fooMock
                .Setup(x => x.Handle(It.IsAny<string>()))
                .Returns((string x) => x)
                .Callback((string _) => called = !called);

            var cachedFoo = CacheInterceptorBuilder.New<IFoo>()
                .WithProxyGenerator(proxyGenerator)
                .WithStore(new SimpleMethodMemoryCache())
                .Build(fooMock.Object);

            cachedFoo.Handle("Hello world!");
            cachedFoo.Handle("Hello world!");
            
            Assert.True(called);
        }
    }
}