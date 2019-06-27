# Simple-proxy-cache

Using AOP and proxy pattern to cache method result (both sync and async) all using attributes over method.

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
var cachedFoo = CacheInterceptorBuilder.New<IFoo>()
  .WithProxyGenerator(proxyGenerator)
  .WithStore(new SimpleMethodMemoryCache())
  .Build(new Foo());
```
