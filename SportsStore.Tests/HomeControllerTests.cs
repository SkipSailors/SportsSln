namespace SportsStore.Tests;

using Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using Models.ViewModels;
using NSubstitute;

[TestClass]
public class HomeControllerTests
{
    [TestMethod]
    public void Can_Use_Repository()
    {
        IStoreRepository mock = Substitute.For<IStoreRepository>();
        IQueryable<Product> data = new[]
        {
            new Product { ProductID = 1, Name = "P1" },
            new Product { ProductID = 2, Name = "P2" }
        }.AsQueryable();
        mock.Products.Returns(data);
        HomeController controller = new(mock);
        ProductListViewModel result = (controller.Index(null) as ViewResult)?
            .ViewData
            .Model as ProductListViewModel ?? new ProductListViewModel();
        Product[] prodArray = result.Products.ToArray() ?? Array.Empty<Product>();
        Assert.IsTrue(prodArray.Length == 2);
        Assert.AreEqual("P1", prodArray[0].Name);
        Assert.AreEqual("P2", prodArray[1].Name);
    }

    [TestMethod]
    public void Can_Paginate()
    {
        IStoreRepository mock = Substitute.For<IStoreRepository>();
        IQueryable<Product> data = new[]
        {
            new Product { ProductID = 1, Name = "P1" },
            new Product { ProductID = 2, Name = "P2" },
            new Product { ProductID = 3, Name = "P3" },
            new Product { ProductID = 4, Name = "P4" },
            new Product { ProductID = 5, Name = "P5" }
        }.AsQueryable();
        mock.Products.Returns(data);
        HomeController controller = new(mock) { PageSize = 3 };
        ProductListViewModel result = (controller.Index(null, 2) as ViewResult)?
            .ViewData
            .Model as ProductListViewModel ?? new ProductListViewModel();
        Product[] prodArray = result?.Products.ToArray();
        Assert.AreEqual(2, prodArray.Length);
        Assert.AreEqual("P4", prodArray[0].Name);
        Assert.AreEqual("P5", prodArray[1].Name);
    }

    [TestMethod]
    public void Can_Send_Pagination_ViewModel()
    {
        IStoreRepository mock = Substitute.For<IStoreRepository>();
        IQueryable<Product> data = new[]
        {
            new Product { ProductID = 1, Name = "P1" },
            new Product { ProductID = 2, Name = "P2" },
            new Product { ProductID = 3, Name = "P3" },
            new Product { ProductID = 4, Name = "P4" },
            new Product { ProductID = 5, Name = "P5" }
        }.AsQueryable();
        mock.Products.Returns(data);
        HomeController controller = new(mock) { PageSize = 3 };
        ProductListViewModel result = (controller.Index(null, 2) as ViewResult)?
            .ViewData
            .Model as ProductListViewModel ?? new ProductListViewModel();
        PagingInfo pageInfo = result.PagingInfo;
        Assert.AreEqual(2, pageInfo.CurrentPage);
        Assert.AreEqual(3, pageInfo.ItemsPerPage);
        Assert.AreEqual(5, pageInfo.TotalItems);
        Assert.AreEqual(2, pageInfo.TotalPages);
    }

    [TestMethod]
    public void Can_Filter_Products()
    {
        IStoreRepository mock = Substitute.For<IStoreRepository>();
        IQueryable<Product> data = new[]
        {
            new Product { ProductID = 1, Name = "P1", Category = "Cat1" },
            new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
            new Product { ProductID = 3, Name = "P3", Category = "Cat1" },
            new Product { ProductID = 4, Name = "P4", Category = "Cat2" },
            new Product { ProductID = 5, Name = "P5", Category = "Cat3" }
        }.AsQueryable();
        mock.Products.Returns(data);
        HomeController controller = new(mock) { PageSize = 3 };
        Product[] result = ((controller
                    .Index("Cat2") as ViewResult)?
                .ViewData
                .Model as ProductListViewModel ?? new ProductListViewModel())
            .Products.ToArray();
        Assert.AreEqual(2, result.Length);
        Assert.AreEqual("P2", result[0].Name);
        Assert.AreEqual("Cat2", result[0].Category);
        Assert.AreEqual("P4", result[1].Name);
        Assert.AreEqual("Cat2", result[1].Category);
    }

    [TestMethod]
    public void Generate_Category_Specific_Product_Count()
    {
        IStoreRepository mock = Substitute.For<IStoreRepository>();
        IQueryable<Product> data = new[]
        {
            new Product { ProductID = 1, Name = "P1", Category = "Cat1" },
            new Product { ProductID = 2, Name = "P2", Category = "Cat2" },
            new Product { ProductID = 3, Name = "P3", Category = "Cat1" },
            new Product { ProductID = 4, Name = "P4", Category = "Cat2" },
            new Product { ProductID = 5, Name = "P5", Category = "Cat3" }
        }.AsQueryable();
        mock.Products.Returns(data);
        HomeController target = new(mock) { PageSize = 3 };
        ProductListViewModel GetModel(ViewResult result) =>
            (result?.ViewData?.Model as ProductListViewModel)!;
        int? res1 = GetModel((target.Index("Cat1") as ViewResult)!)?.PagingInfo.TotalItems;
        int? res2 = GetModel((target.Index("Cat2") as ViewResult)!)?.PagingInfo.TotalItems;
        int? res3 = GetModel((target.Index("Cat3") as ViewResult)!)?.PagingInfo.TotalItems;
        int? resAll = GetModel((target.Index(null) as ViewResult)!)?.PagingInfo.TotalItems;
        Assert.AreEqual(2, res1);
        Assert.AreEqual(2, res2);
        Assert.AreEqual(1, res3);
        Assert.AreEqual(5, resAll);
    }
}