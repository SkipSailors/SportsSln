namespace SportsStore.Tests;

using Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using NSubstitute;

[TestClass]
public class NavigationMenuViewComponentTests
{
    [TestMethod]
    public void Can_Select_Categories()
    {
        IStoreRepository mock = Substitute.For<IStoreRepository>();
        IQueryable<Product> data = new[]
        {
            new Product { ProductID = 1, Name = "P1", Category = "Apples" },
            new Product { ProductID = 2, Name = "P2", Category = "Apples" },
            new Product { ProductID = 3, Name = "P3", Category = "Plums" },
            new Product { ProductID = 4, Name = "P4", Category = "Oranges" },
        }.AsQueryable();
        mock.Products.Returns(data);
        NavigationMenuViewComponent target = new(mock);
        string[] results = ((IEnumerable<string>?)(target.Invoke() as ViewViewComponentResult)?
                            .ViewData?
                            .Model ?? Enumerable.Empty<string>())
            .ToArray();
        Assert.IsTrue(results.SequenceEqual( new [] {"Apples", "Oranges", "Plums"  }));
    }

    [TestMethod]
    public void Indicates_Selected_Category()
    {
        const string categoryToSelect = "Apples";
        IStoreRepository mock = Substitute.For<IStoreRepository>();
        IQueryable<Product> data = new[]
        {
            new Product { ProductID = 1, Name = "P1", Category = "Apples" },
            new Product { ProductID = 2, Name = "P2", Category = "Oranges" },
        }.AsQueryable();
        NavigationMenuViewComponent target = new(mock);
        target.ViewComponentContext = new()
        {
            ViewContext = new ViewContext
            {
                RouteData = new Microsoft.AspNetCore.Routing.RouteData()
            }
        };
        target.RouteData.Values["category"] = categoryToSelect;
        string? result = (string?)(target.Invoke() as ViewViewComponentResult)?
            .ViewData?["SelectedCategory"];
        Assert.AreEqual(categoryToSelect, result);
    }
}