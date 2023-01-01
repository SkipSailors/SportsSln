namespace SportsStore.Controllers;

using Microsoft.AspNetCore.Mvc;
using Models;

public class OrderController : Controller
{
    private readonly Cart cart;

    private readonly IOrderRepository repository;

    public OrderController(
        IOrderRepository repoService,
        Cart cartService)
    {
        repository = repoService;
        cart = cartService;
    }

    public ViewResult Checkout()
    {
        return View(new Order());
    }

    [HttpPost]
    public IActionResult Checkout(Order order)
    {
        if (!cart.Lines.Any())
        {
            ModelState.AddModelError("", "Sorry, your cart is empty");
        }

        if (ModelState.IsValid)
        {
            order.Lines = cart.Lines.ToArray();
            repository.SaveOrder(order);
            cart.Clear();
            return RedirectToPage("/Completed", new { orderId = order.OrderID });
        }

        return View();
    }
}