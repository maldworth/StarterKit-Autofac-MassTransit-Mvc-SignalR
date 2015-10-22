namespace StarterKit.Web.Controllers
{
    using MassTransit;
    using StarterKit.Contracts;
    using StarterKit.Web.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Mvc;

    public class HomeController : Controller
    {
        private readonly IBus _bus;

        public HomeController(IBus bus)
        {
            _bus = bus;
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Submit(MyMessageViewModel model)
        {
            await _bus.Publish<MyMessage>(new
            {
                Message = model.Text ?? "Unknown"
            });

            return View("Index");
        }
    }
}