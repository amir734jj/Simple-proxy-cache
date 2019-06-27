# Simple-proxy-cache

[NuGet](https://www.nuget.org/packages/SimpleProxyCache/)

### Description
Using AOP and proxy pattern to cache method result (both sync and async) all using attributes over method. Under the hood, this library will cache result of method given a sequence of arguments. So if we call the method with the same sequence of arguments then we just use the cached result.

### Example
Add `[CacheResult]` or `[InvalidateCache]` attribute over interface methods:

```csharp
public interface IFoo
{
    [CacheResult]
    string Get(string id);

    [CacheResult]
    Task<string> GetAsync(string id);
    
    [InvalidateCache]
    void Delete(string id);

    [InvalidateCache]
    Task DeleteAsync(string id);
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
cachedFoo.Get("Hello world!");

// The target method will not get called because we cached this method with "Hello world!" argument
cachedFoo.Get("Hello world!");
```

### Notes
- Note that if method does not have any attributes then it will not be proxied or caches.
