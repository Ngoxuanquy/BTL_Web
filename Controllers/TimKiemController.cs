using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;

namespace BTL_Web.Controllers
{
    public class TimKiemController : Controller
    {
        private readonly BtlWebNcContext _DbContext;

        public TimKiemController(BtlWebNcContext _dbContext)
        {
            this._DbContext = _dbContext;
        }

        // [HttpGet("GetAll")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Search(string searchString)
        {
            var products = this._DbContext.Products.ToList();

            var prod = products.Where(s => s.ProductDes!.Contains(searchString));

            return Json(new { data = prod });
        }

    }
}
