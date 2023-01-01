namespace SportsStore.Tests;

using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Models;
using NSubstitute;
using Pages;

[TestClass]
public class CartPageTests
{
    [TestMethod]
    public void Can_Load_Cart()
    {
        Product p1 = new() { ProductID = 1, Name = "P1" };
        Product p2 = new() { ProductID = 2, Name = "P2" };
        IQueryable<Product> list = new[] { p1, p2 }.AsQueryable();
        IStoreRepository mockRepo = Substitute.For<IStoreRepository>();
        mockRepo.Products.Returns(list);
        Cart testCart = new();
        testCart.AddItem(p1, 2);
        testCart.AddItem(p2, 1);
        ISession mockSession = Substitute.For<ISession>();
        byte[] data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(testCart));
        mockSession
            .TryGetValue(Arg.Any<string>(), out Arg.Any<byte[]?>())
            .Returns(x =>
            {
                x[1] = data;
                return true;
            });
        HttpContext mockContext = Substitute.For<HttpContext>();
        mockContext.Session.Returns(mockSession);
        CartModel cartModel = new(mockRepo, testCart)
        {
            PageContext = new PageContext(new ActionContext
            {
                HttpContext = mockContext,
                RouteData = new RouteData(),
                ActionDescriptor = new PageActionDescriptor()
            })
        };
        cartModel.OnGet("myUrl");
        Assert.AreEqual(2, cartModel.Cart?.Lines.Count);
        Assert.AreEqual("myUrl", cartModel.ReturnUrl);
    }

    [TestMethod]
    public void Can_Update_Cart()
    {
        Product p1 = new() { ProductID = 1, Name = "P1" };
        IQueryable<Product> list = new[] { p1 }.AsQueryable();
        IStoreRepository mockRepo = Substitute.For<IStoreRepository>();
        mockRepo.Products.Returns(list);
        Cart? testCart = new();
        ISession mockSession = Substitute.For<ISession>();
        mockSession.Set(Arg.Any<string>(), Arg.Do<byte[]>(SessionCallback));
        HttpContext mockContext = Substitute.For<HttpContext>();
        mockContext.Session.Returns(mockSession);
        CartModel cartModel = new(mockRepo, testCart)
        {
            PageContext = new PageContext(new ActionContext
            {
                HttpContext = mockContext,
                RouteData = new RouteData(),
                ActionDescriptor = new PageActionDescriptor()
            })
        };
        cartModel.OnPost(1, "myUrl");
        Assert.AreEqual(1, testCart.Lines.Count);
        Assert.AreEqual("P1", testCart.Lines.First().Product.Name);
        Assert.AreEqual(1, testCart.Lines.First().Quantity);

        void SessionCallback(byte[] val)
        {
            testCart = JsonSerializer.Deserialize<Cart>(Encoding.UTF8.GetString(val));
        }
    }
}