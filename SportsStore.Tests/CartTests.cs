namespace SportsStore.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Models;

[TestClass]
public class CartTests
{
    [TestMethod]
    public void Can_Add_New_Lines()
    {
        Product p1 = new() { ProductID = 1, Name = "P1" };
        Product p2 = new() { ProductID = 2, Name = "P2" };
        Cart target = new();
        target.AddItem(p1, 1);
        target.AddItem(p2, 1);
        CartLine[] results = target.Lines.ToArray();
        Assert.AreEqual(2, results.Length);
        Assert.AreEqual(p1, results[0].Product);
        Assert.AreEqual(p2, results[1].Product);
    }

    [TestMethod]
    public void Can_Add_Quantity_For_Existing_Lines()
    {
        Product p1 = new() { ProductID = 1, Name = "P1" };
        Product p2 = new() { ProductID = 2, Name = "P2" };
        Cart target = new();
        target.AddItem(p1, 1);
        target.AddItem(p2, 1);
        target.AddItem(p1, 10);
        CartLine[] results = (target.Lines ?? new List<CartLine>())
            .OrderBy(c => c.Product.ProductID)
            .ToArray();
        Assert.AreEqual(2, results.Length);
        Assert.AreEqual(11, results[0].Quantity);
        Assert.AreEqual(1, results[1].Quantity);
    }

    [TestMethod]
    public void Can_Remove_Line()
    {
        Product p1 = new() { ProductID = 1, Name = "P1" };
        Product p2 = new() { ProductID = 2, Name = "P2" };
        Product p3 = new() { ProductID = 3, Name = "P3" };
        Cart target = new();
        target.AddItem(p1, 1);
        target.AddItem(p2, 3);
        target.AddItem(p3, 5);
        target.AddItem(p1, 1);
        target.RemoveLine(p2);
        Assert.AreEqual(0, target.Lines.Count(c => c.Product == p2));
        Assert.AreEqual(2, target.Lines.Count);
    }

    [TestMethod]
    public void Calculate_Cart_Total()
    {
        Product p1 = new() { ProductID = 1, Name = "P1", Price = 100M };
        Product p2 = new() { ProductID = 2, Name = "P2", Price = 50M };
        Cart target = new();
        target.AddItem(p1, 1);
        target.AddItem(p2, 1);
        target.AddItem(p1, 3);
        decimal result = target.ComputeTotalValue();
        Assert.AreEqual(450M, result);
    }

    [TestMethod]
    public void Can_Clear_Contents()
    {
        Product p1 = new() { ProductID = 1, Name = "P1", Price = 100M };
        Product p2 = new() { ProductID = 2, Name = "P2", Price = 50M };
        Cart target = new();
        target.AddItem(p1, 1);
        target.AddItem(p2, 1);
        target.Clear();
        Assert.AreEqual(0, target.Lines.Count);
    }
}