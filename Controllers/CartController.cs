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

                int totalProductQuantity = this._DbContext.Orders
                    .Where(o => o.Status == "Đặt hàng" && o.UserId == intValue && o.SoLuong.HasValue)
                    .Sum(o => o.SoLuong.Value);

                Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                });

                // int value = _DbContext.Orders
                //         .Where(p => p.UserId == intValue && p.Status == "Đặt hàng")
                //         .Count();

                // Console.WriteLine("KeyValuePair" + value);

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
            if (userId == "")
            {

            }
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
                return RedirectToAction("", "Login");

            }
        }


        public IActionResult DonHangDaDong()
        {
            var userId = Request.Cookies["id"];
            if (userId == "")
            {

            }
            if (int.TryParse(userId, out int intValue))
            {
                var ordersWithProducts = this._DbContext.Orders
                    .Where(p => p.UserId == intValue && p.Status == "Đã đóng hàng")
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
                return RedirectToAction("", "Login");
                return BadRequest("Invalid user ID.");

            }
        }


        [HttpPost]
        public JsonResult SoLuongTru(int id)
        {

            var userId = Request.Cookies["id"];

            var product = this._DbContext.Orders.Find(id);

            if (product != null)
            {
                // Assuming there is a property like "SoLuong" in your product entity
                product.SoLuong--; // Increase the quantity

                // Save changes to the database
                this._DbContext.SaveChanges();
            }

            if (int.TryParse(userId, out int intValue))
            {

                int totalProductQuantity = this._DbContext.Orders
                    .Where(o => o.Status == "Đặt hàng" && o.UserId == intValue && o.SoLuong.HasValue)
                    .Sum(o => o.SoLuong.Value);

                Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                });

            }
            // Redirect to a different action or view if needed
            return Json(new
            {
                product
            });

        }

        [HttpPost]
        public IActionResult SoLuongCong(int id)
        {
            var userId = Request.Cookies["id"];

            Console.WriteLine(userId + "userId userId userId");

            var product = this._DbContext.Orders.Find(id);

            if (product != null)
            {
                // Assuming there is a property like "SoLuong" in your product entity
                product.SoLuong++; // Increase the quantity

                // Save changes to the database
                this._DbContext.SaveChanges();
            }

            if (int.TryParse(userId, out int intValue))
            {

                int totalProductQuantity = this._DbContext.Orders
                    .Where(o => o.Status == "Đặt hàng" && o.UserId == intValue && o.SoLuong.HasValue)
                    .Sum(o => o.SoLuong.Value);

                Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                });

            }

            // Redirect to a different action or view if needed
            return Json(new { product });
        }




        public IActionResult Privacy()
        {
            return View();
        }
        public ActionResult CreateOrder(int id)
        {
            var userIdCookie = Request.Cookies["id"];

            if (string.IsNullOrEmpty(userIdCookie))
            {
                TempData["ErrorMessage"] = "Bạn vui lòng đăng nhập để mua hàng!!!";
                return RedirectToAction("", "Login");
            }

            if (int.TryParse(userIdCookie, out int userId))
            {
                try
                {
                    // Check if the user already has the product in the order
                    var existingOrder = _DbContext.Orders
                        .SingleOrDefault(o => o.ProductId == id && o.UserId == userId && o.Status == "Đặt hàng");

                    if (existingOrder != null)
                    {
                        // If the product already exists, increase the quantity
                        existingOrder.SoLuong += 1;
                    }
                    else
                    {
                        // If the product does not exist, create a new order
                        var newOrder = new Order
                        {
                            ProductId = id,
                            UserId = userId,
                            Adrees = "a", // Corrected typo "Adrees" to "Address"
                            Number = "01",
                            Status = "Đặt hàng",
                            SoLuong = 1
                        };

                        _DbContext.Orders.Add(newOrder); // Add the new order to the Orders DbSet
                    }

                    _DbContext.SaveChanges(); // Save changes to the database context

                    // Update the total product quantity in the cookie
                    int totalProductQuantity = _DbContext.Orders
                        .Where(o => o.Status == "Đặt hàng" && o.UserId == userId && o.SoLuong.HasValue)
                        .Sum(o => o.SoLuong ?? 0);

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


        [HttpDelete]
        public JsonResult DeleteCart(int id)
        {
            try
            {

                var userId = Request.Cookies["id"];

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
                if (int.TryParse(userId, out int intValue))
                {

                    int totalProductQuantity = this._DbContext.Orders
                        .Where(o => o.Status == "Đặt hàng" && o.UserId == intValue && o.SoLuong.HasValue)
                        .Sum(o => o.SoLuong.Value);

                    Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
                    {
                        Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                    });

                }

                return Json(new { success = id });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, ghi log hoặc trả về thông báo lỗi.
                return Json(new { error = "An error occurred: " + ex.Message });
            }
        }

        public class OrderUpdateModel
        {
            public List<int> OrderIds { get; set; }
            public string FullName { get; set; }
            public string Address { get; set; }
            public string PhoneNumber { get; set; }
        }

        [HttpPost]
        public JsonResult DatHang([FromBody] OrderUpdateModel value)
        {
            try
            {
                // Kiểm tra nếu danh sách value.orderIds không rỗng
                if (value.OrderIds != null)
                {
                    // Lặp qua từng value.orderIds trong danh sách
                    foreach (var orderId in value.OrderIds)
                    {
                        Console.WriteLine("Processing orderId: " + orderId);

                        // Tìm đơn đặt hàng có value.orderIds trùng với id
                        var order = this._DbContext.Orders.FirstOrDefault(row => row.OrderId == orderId);

                        Console.WriteLine("Found order: " + order);

                        // Kiểm tra xem đơn đặt hàng có tồn tại không
                        if (order != null)
                        {
                            // Thực hiện cập nhật trạng thái đơn đặt hàng, ví dụ: đặt hàng thành công
                            order.Status = "Completed";
                            order.Adrees = value.Address;
                            order.Number = value.PhoneNumber;
                            // Cập nhật các thuộc tính khác nếu cần thiết

                            // Lưu thay đổi vào cơ sở dữ liệu
                            this._DbContext.SaveChanges();
                        }
                    }
                }

                // Trả về một JSON object để thể hiện rằng thao tác đã thành công
                return Json(new { success = true, order = value });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, ghi log hoặc trả về thông báo lỗi.
                return Json(new { error = "An error occurred: " + ex.Message });
            }
        }



        [HttpPost]
        public JsonResult DongHang(int orderId)
        {
            try
            {
                // Kiểm tra nếu danh sách value.orderIds không rỗng

                Console.WriteLine("Processing orderId: " + orderId);

                // Tìm đơn đặt hàng có value.orderIds trùng với id
                var order = this._DbContext.Orders.FirstOrDefault(row => row.OrderId == orderId);

                Console.WriteLine("Found order: " + order);

                // Kiểm tra xem đơn đặt hàng có tồn tại không
                if (order != null)
                {
                    // Thực hiện cập nhật trạng thái đơn đặt hàng, ví dụ: đặt hàng thành công
                    order.Status = "Đã đóng hàng";

                    this._DbContext.SaveChanges();
                }

                // Trả về một JSON object để thể hiện rằng thao tác đã thành công
                return Json(new { success = true, order = orderId });
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ, ghi log hoặc trả về thông báo lỗi.
                return Json(new
                {
                    error = "An error occurred: " + ex.Message
                });
            }
        }





        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
