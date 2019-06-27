# Simple-proxy-cache

Using AOP and proxy pattern to cache method result (both sync and async) all using attributes over method. Under the hood, this library will cache result of method given a sequence of arguments. So if we call the method with the same sequence of arguments then we just use the cached result.

Add `[CacheResult]` or `[InvalidateCache]` attribute over interface methods:

```csharp
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
```

Create a cache proxy instance (use `SimpleMethodMemoryCache` or an interface that implements `ICacheMethodUtility`)
```csharp
// Re-use the same instance
var proxyGenerator = new ProxyGenerator();

// Create instance of Foo
Foo target = new Foo();

// Create cached proxy instance
var cachedFoo = CacheInterceptorBuilder.New<IFoo>()
  .WithProxyGenerator(proxyGenerator)
  .WithStore(new SimpleMethodMemoryCache())
  .Build(target);
  
// This will call the target method
cachedFoo.Handle("Hello world!");

// The target method will not get called because we cached this method with "Hello world!" argument
cachedFoo.Handle("Hello world!");
```
