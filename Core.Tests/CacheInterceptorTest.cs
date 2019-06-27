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
        
        [InvalidateCache]
        void Delete();

        [InvalidateCache]
        Task DeleteAsync();
    }
    
    public class CacheInterceptorTest
    {
        [Fact]
        public void Test__CacheSync()
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
        
        [Fact]
        public async Task Test__CacheAsync()
        {
            var proxyGenerator = new ProxyGenerator();

            var called = false;
            
            var fooMock = new Mock<IFoo>();
            fooMock
                .Setup(x => x.HandleAsync(It.IsAny<string>()))
                .ReturnsAsync((string x) => x)
                .Callback((string _) => called = !called);

            var cachedFoo = CacheInterceptorBuilder.New<IFoo>()
                .WithProxyGenerator(proxyGenerator)
                .WithStore(new SimpleMethodMemoryCache())
                .Build(fooMock.Object);

            await cachedFoo.HandleAsync("Hello world!");
            await cachedFoo.HandleAsync("Hello world!");
            
            Assert.True(called);
        }
        
        [Fact]
        public void Test__InvalidateSync()
        {
            var proxyGenerator = new ProxyGenerator();

            var called = false;
            
            var fooMock = new Mock<IFoo>();
            fooMock
                .Setup(x => x.HandleAsync(It.IsAny<string>()))
                .ReturnsAsync((string x) => x)
                .Callback((string _) => called = !called);
            
            fooMock
                .Setup(x => x.Delete())
                .Callback(() => called = !called);

            var cachedFoo = CacheInterceptorBuilder.New<IFoo>()
                .WithProxyGenerator(proxyGenerator)
                .WithStore(new SimpleMethodMemoryCache())
                .Build(fooMock.Object);

            cachedFoo.Handle("Hello world!");
            cachedFoo.Delete();
            cachedFoo.Handle("Hello world!");
            
            Assert.True(called);
        }
        
        [Fact]
        public async Task Test__InvalidateAsync()
        {
            var proxyGenerator = new ProxyGenerator();

            var called = false;
            
            var fooMock = new Mock<IFoo>();
            fooMock
                .Setup(x => x.HandleAsync(It.IsAny<string>()))
                .ReturnsAsync((string x) => x)
                .Callback((string _) => called = !called);
            
            fooMock
                .Setup(x => x.DeleteAsync())
                .Returns(() => Task.CompletedTask)
                .Callback(() => called = !called);

            var cachedFoo = CacheInterceptorBuilder.New<IFoo>()
                .WithProxyGenerator(proxyGenerator)
                .WithStore(new SimpleMethodMemoryCache())
                .Build(fooMock.Object);

            await cachedFoo.HandleAsync("Hello world!");
            await cachedFoo.DeleteAsync();
            await cachedFoo.HandleAsync("Hello world!");
            
            Assert.True(called);
        }
    }
}