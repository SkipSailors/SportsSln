namespace SportsStore.Tests;

using Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Models;
using NSubstitute;
using SportsStore.Migrations;

[TestClass]
public class OrderControllerTests
{
    [TestMethod]
    public void Cannot_Checkout_Empty_Cart()
    {
        IOrderRepository mock = Substitute.For<IOrderRepository>();
        Cart cart = new();
        Order order = new();
        OrderController target = new(mock, cart);
        ViewResult? result = target.Checkout(order) as ViewResult;
        mock.DidNotReceive().SaveOrder(Arg.Any<Order>());
        Assert.IsTrue(string.IsNullOrWhiteSpace(result!.ViewName));
        Assert.IsFalse(result!.ViewData.ModelState.IsValid);
    }

    [TestMethod]
    public void Cannot_Checkout_Invalid_ShippingDetails()
    {
        IOrderRepository mock = Substitute.For<IOrderRepository>();
        Cart cart = new();
        cart.AddItem(new(), 1);
        OrderController target = new(mock, cart);
        target.ModelState.AddModelError("error", "error");
        ViewResult? result = target.Checkout(new()) as ViewResult;
        mock.DidNotReceive().SaveOrder(Arg.Any<Order>());
        Assert.IsTrue(string.IsNullOrWhiteSpace(result!.ViewName));
        Assert.IsFalse(result!.ViewData.ModelState.IsValid);
    }

    [TestMethod]
    public void Can_Checkout_And_Submit_Order()
    {
        IOrderRepository mock = Substitute.For<IOrderRepository>();
        Cart cart = new();
        cart.AddItem(new(), 1);
        OrderController target = new(mock, cart);
        RedirectToPageResult? result = target.Checkout(new Order()) as RedirectToPageResult;
        mock.Received(1).SaveOrder(Arg.Any<Order>());
        Assert.AreEqual("/Completed", result?.PageName);
    }
}