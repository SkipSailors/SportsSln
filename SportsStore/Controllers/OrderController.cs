namespace SportsStore.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Models;

    public class OrderController: Controller
    {
        public ViewResult Checkout() => View(new Order());
    }
}
