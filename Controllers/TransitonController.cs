using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;

namespace BTL_Web.Controllers
{
    public class TransitionController : Controller
    {
        private readonly BtlWebNcContext _DbContext;

        public TransitionController(BtlWebNcContext _dbContext)
        {
            this._DbContext = _dbContext;
        }

        // [HttpGet("GetAll")]
        public IActionResult Index()
        {
            var users = this._DbContext.Products.ToList();
            return View(users);
        }
        // [HttpPost]
        // public ActionResult CreateTransition(int UserId, List<int> OrderId)
        // {
        //     // Create a new order
        //     try
        //     {
        //         var newOrder = new Transition
        //         {
        //             OrderId = OrderId,
        //             // Set other properties as needed
        //             UserId = UserId, // Set the UserId to the parsed user ID
        //             Diachi = "a", // Corrected typo "Adrees" to "Address"
        //             Sodienthoai = "01",
        //             Name = "Đặt hàng",
        //         };

        //         _DbContext.Transitions.Add(newOrder); // Add the new order to the Orders DbSet
        //         _DbContext.SaveChanges(); // Save changes to the database context

        //         int itemCount = _DbContext.Orders.Count();

        //         Response.Cookies.Append("soluong", itemCount.ToString(), new CookieOptions
        //         {
        //             Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
        //         });
        //         // Optionally, you can redirect to the appropriate action or view after the order is created
        //         return RedirectToAction("Index", "Cart");
        //     }
        //     catch (Exception ex)
        //     {
        //         TempData["ErrorMessage"] = "Lỗi khi lưu đơn đặt hàng. Vui lòng thử lại sau. " + ex.Message;
        //         // Log the exception for further investigation
        //         // _logger.LogError(ex, "An error occurred while saving the order data.");
        //         Console.WriteLine(ex);
        //         return View();
        //     }

        // }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
