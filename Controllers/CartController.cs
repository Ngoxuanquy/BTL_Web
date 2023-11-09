using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using System;
using Microsoft.Extensions.Logging;

namespace BTL_Web.Controllers
{
    public class CartController : Controller
    {
        private readonly BtlWebNcContext _DbContext;
        private readonly ILogger<BtlWebNcContext> _logger;
        public CartController(BtlWebNcContext _dbContext, ILogger<BtlWebNcContext> logger)
        {
            this._DbContext = _dbContext;
            _logger = logger;
        }

        public class OrderProductDTO
        {
            public int OrderId { get; set; }
            public string ProductName { get; set; }
            public string ProductImg { get; set; }
            public int ProductPrice { get; set; }
            public string ProductDes { get; set; }

            // Các thuộc tính khác mà bạn cần
        }
        // [HttpGet("GetAll")]
        public IActionResult Index()
        {
            var userId = Request.Cookies["id"];
            if (int.TryParse(userId, out int intValue))
            {
                var ordersWithProducts = this._DbContext.Orders
                    .Where(p => p.UserId == intValue)
                    .Join(
                        this._DbContext.Products,
                        order => order.ProductId,
                        product => product.ProductId,
                        (order, product) => new OrderProductDTO
                        {
                            OrderId = order.OrderId,
                            ProductName = product.ProductName,
                            ProductDes = product.ProductDes,
                            ProductImg = product.ProductImg,
                            // ProductPrice = product.ProductPrice ?? 0,
                            // Other properties you may need
                        })
                    .ToList();

                return View(ordersWithProducts);
            }
            else
            {
                // Handle the case where conversion fails
                // For example, you can return an error view or handle it as per your application's requirements
                return BadRequest("Invalid user ID.");
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }
        public ActionResult CreateOrder(int id)
        {
            // Create a new order
            var userId = Request.Cookies["id"];
            if (userId == "" || userId == null)
            {
                TempData["ErrorMessage"] = "Bạn vui lòng đăng nhập để mua hàng!!!";
                return RedirectToAction("", "Login");

            }
            else
            {
                if (int.TryParse(userId, out int intValue))
                {
                    try
                    {
                        var newOrder = new Order
                        {
                            ProductId = id,
                            // Set other properties as needed
                            UserId = intValue, // Set the UserId to the parsed user ID
                            Adrees = "a", // Corrected typo "Adrees" to "Address"
                            Number = "01",
                            Status = "Đặt hàng"
                        };

                        _DbContext.Orders.Add(newOrder); // Add the new order to the Orders DbSet
                        _DbContext.SaveChanges(); // Save changes to the database context

                        // Optionally, you can redirect to the appropriate action or view after the order is created
                        return RedirectToAction("Index", "Cart");
                    }
                    catch (Exception ex)
                    {
                        TempData["ErrorMessage"] = "Lỗi khi lưu đơn đặt hàng. Vui lòng thử lại sau. " + ex.Message;
                        // Log the exception for further investigation
                        // _logger.LogError(ex, "An error occurred while saving the order data.");
                        Console.WriteLine(ex);
                        return View();
                    }
                }

                // Handle the case where the user ID parsing fails
                TempData["ErrorMessage"] = "Lỗi khi lưu đơn đặt hàng. Vui lòng thử lại sau.";
                return View();
            }
        }

        [HttpDelete]
        public JsonResult DeleteCart(int id)
        {
            try
            {
                var tblChitiethoadon = this._DbContext.Orders.Where(row => row.ProductId == id).ToList();
                Console.WriteLine(tblChitiethoadon);
                this._DbContext.Orders.RemoveRange(tblChitiethoadon);

                Console.WriteLine("abccc");

                var tblHanghoa = this._DbContext.Orders.SingleOrDefault(row => row.ProductId == id);
                if (tblHanghoa != null)
                {
                    this._DbContext.Orders.Remove(tblHanghoa);
                }

                this._DbContext.SaveChanges();

                return Json(new { success = id });
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error message.
                return Json(new { error = "An error occurred: " + ex.Message });
            }
        }



        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
