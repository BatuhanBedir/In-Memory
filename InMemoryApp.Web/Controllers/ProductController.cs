using InMemoryApp.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace InMemoryApp.Web.Controllers;

public class ProductController : Controller
{
    private IMemoryCache _memoryCache;
    public ProductController(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }
    public IActionResult SetCache()
    {
        MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();

        options.AbsoluteExpiration = DateTime.Now.AddSeconds(10);
        //options.AbsoluteExpiration = DateTime.Now.AddMicroseconds(1);
        //options.SlidingExpiration = TimeSpan.FromSeconds(10);

        options.Priority = CacheItemPriority.Normal;

        options.RegisterPostEvictionCallback((key, value, reason, state) =>
        {
            _memoryCache.Set("callback", $"{key}-->{value}=>reason:{reason}");
        });

        Product product = new Product { Id = 1, Name = "Kalem", Price = 200 };
        _memoryCache.Set<Product>("product:1", product);//otomatik serialize.
        
        _memoryCache.Set<string>("date", DateTime.Now.ToString(), options);

        return View();
    }
    public IActionResult GetCache()
    {
        #region Remove
        //_memoryCache.Remove("date"); 
        #endregion

        #region GetOrCreate
        //_memoryCache.GetOrCreate<string>("date", entry =>
        //{
        //    entry.AbsoluteExpiration = DateTime.Now.AddSeconds(30);
        //    entry.SlidingExpiration = TimeSpan.FromSeconds(5);
        //    return DateTime.Now.ToString();
        //}); 
        #endregion

        _memoryCache.TryGetValue("date", out string date);
        _memoryCache.TryGetValue<string>("callback", out string callback);
        ViewBag.date = date;
        ViewBag.callback = callback;

        ViewBag.product = _memoryCache.Get<Product>("product:1");
        return View();
    }
}
