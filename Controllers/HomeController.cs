using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;

namespace BTL_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly BtlWebNcContext _DbContext;

        public HomeController(BtlWebNcContext _dbContext)
        {
            this._DbContext = _dbContext;
        }

    // [HttpGet("GetAll")]
        public IActionResult Index()
        {
            var users = this._DbContext.Products.ToList();
            return View(users);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //  public ActionResult ProductDetail(int id)
        //     {
        //         var product = this._DbContext.Products.FirstOrDefault(p => p.Id == id);
        //           if (product == null)
        //     {
        //         return NotFound(); // Or any other action you want to take if the product doesn't exist
        //     }
        //          return View(product);
        //             // return RedirectToAction("Index", "Login");
        //         //      TempData["ProductData"] = product; // Storing the product data in TempData
        //         // return RedirectToAction("ProductDetail", "Product");

        //     }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
