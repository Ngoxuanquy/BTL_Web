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
            public int SoLuong { get; set; }


            // Các thuộc tính khác mà bạn cần
        }
        // [HttpGet("GetAll")]
        public IActionResult Index()
        {
            var userId = Request.Cookies["id"];
            if (int.TryParse(userId, out int intValue))
            {
                var ordersWithProducts = this._DbContext.Orders
                    .Where(p => p.UserId == intValue && p.Status == "Đặt hàng")
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
                            SoLuong = (int)order.SoLuong,
                            ProductPrice = (int)product.ProductPrice,
                            // Other properties you may need
                        })
                    .ToList();

                int totalProductQuantity = _DbContext.Orders
                                  .Where(o => o.Status == "Đặt hàng") // Kiểm tra SoLuong có giá trị
                                  .Sum(o => o.SoLuong ?? 0);

                Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                });


                return View(ordersWithProducts);
            }
            else
            {
                // Handle the case where conversion fails
                // For example, you can return an error view or handle it as per your application's requirements
                return BadRequest("Invalid user ID.");
            }
        }

        public IActionResult GetDonHang()
        {
            var userId = Request.Cookies["id"];
            if (int.TryParse(userId, out int intValue))
            {
                var ordersWithProducts = this._DbContext.Orders
                    .Where(p => p.UserId == intValue && p.Status == "Completed")
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
                            SoLuong = (int)order.SoLuong,
                            ProductPrice = (int)product.ProductPrice,
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

        [HttpPost]
        public JsonResult SoLuongTru(int id)
        {
            Console.WriteLine("issnjc");
            Console.WriteLine(id);

            var product = this._DbContext.Orders.Find(id);

            if (product != null)
            {
                // Assuming there is a property like "SoLuong" in your product entity
                product.SoLuong--; // Increase the quantity

                // Save changes to the database
                this._DbContext.SaveChanges();
            }

            int totalProductQuantity = (int)_DbContext.Orders.Sum(o => o.SoLuong);

            Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
            });

            // Redirect to a different action or view if needed
            return Json(new { product });

        }

        [HttpPost]
        public IActionResult SoLuongCong(int id)
        {
            var product = this._DbContext.Orders.Find(id);

            if (product != null)
            {
                // Assuming there is a property like "SoLuong" in your product entity
                product.SoLuong++; // Increase the quantity

                // Save changes to the database
                this._DbContext.SaveChanges();
            }

            int totalProductQuantity = (int)_DbContext.Orders.Sum(o => o.SoLuong);

            Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
            });

            // Redirect to a different action or view if needed
            return RedirectToAction("Index");
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
                            Status = "Đặt hàng",
                            SoLuong = 1
                        };

                        _DbContext.Orders.Add(newOrder); // Add the new order to the Orders DbSet
                        _DbContext.SaveChanges(); // Save changes to the database context

                        int totalProductQuantity = (int)_DbContext.Orders.Sum(o => o.SoLuong);

                        Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                        });

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
                // Tìm tất cả các hàng hóa trong giỏ hàng có ProductId trùng với id
                var cartItems = this._DbContext.Orders.Where(row => row.OrderId == id).ToList();

                // Kiểm tra nếu có mục nào trong giỏ hàng thì mới tiến hành xóa
                if (cartItems.Count > 0)
                {
                    // Xóa tất cả các mục trong giỏ hàng có ProductId trùng với id
                    this._DbContext.Orders.RemoveRange(cartItems);

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    this._DbContext.SaveChanges();
                }

                // int itemCount = _DbContext.Orders.Count();
                int totalProductQuantity = (int)_DbContext.Orders.Where(o => o.Status == "Đặt hàng").Sum(o => o.SoLuong);

                Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                });

                return Json(new { success = id });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, ghi log hoặc trả về thông báo lỗi.
                return Json(new { error = "An error occurred: " + ex.Message });
            }
        }


        [HttpPost]
        public JsonResult DatHang([FromBody] int[] orderIds)
        {
            try
            {
                Console.WriteLine("orderIds111");
                Console.WriteLine(orderIds);


                // Kiểm tra nếu danh sách orderIds không rỗng
                if (orderIds != null && orderIds.Length > 0)
                {
                    // Lặp qua từng orderIds trong danh sách
                    foreach (var orderId in orderIds)
                    {
                        Console.WriteLine("Processing orderId: " + orderId);

                        // Tìm đơn đặt hàng có orderIds trùng với id
                        var order = this._DbContext.Orders.FirstOrDefault(row => row.OrderId == orderId);

                        Console.WriteLine("Found order: " + order);

                        // Kiểm tra xem đơn đặt hàng có tồn tại không
                        if (order != null)
                        {
                            // Thực hiện cập nhật trạng thái đơn đặt hàng, ví dụ: đặt hàng thành công
                            order.Status = "Completed";
                            // Cập nhật các thuộc tính khác nếu cần thiết

                            // Lưu thay đổi vào cơ sở dữ liệu
                            this._DbContext.SaveChanges();
                        }
                    }
                }

                // Trả về một JSON object để thể hiện rằng thao tác đã thành công
                return Json(new { success = true, order = orderIds });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, ghi log hoặc trả về thông báo lỗi.
                return Json(new { error = "An error occurred: " + ex.Message });
            }
        }





        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
