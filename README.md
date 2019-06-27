# Simple-proxy-cache

Using AOP and proxy pattern to cache method result (both sync and async) all using attributes over method.

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

```csharp
var cachedFoo = CacheInterceptorBuilder.New<IFoo>()
  .WithProxyGenerator(proxyGenerator)
  .WithStore(new SimpleMethodMemoryCache())
  .Build(new Foo());
```
