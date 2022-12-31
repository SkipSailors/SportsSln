namespace SportsStore.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

public class HomeController : Controller
{
    private readonly IStoreRepository repository;
    public int PageSize = 4;

    public HomeController(IStoreRepository repo)
    {
        repository = repo;
    }

    public IActionResult Index(string? category, int productPage = 1)
    {
        ProductListViewModel model = new()
        {
            Products = repository
                .Products
                .Where(p => category == null || p.Category == category)
                .OrderBy(p => p.ProductID)
                .Skip((productPage - 1) * PageSize)
                .Take(PageSize),
            PagingInfo = new PagingInfo
            {
                CurrentPage = productPage,
                ItemsPerPage = PageSize,
                TotalItems = category == null
                    ? repository.Products.Count()
                    : repository.Products.Count(e => e.Category == category)
            },
            CurrentCategory = category
        };
        return View(model);
    }
}