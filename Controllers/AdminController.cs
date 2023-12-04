using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;

namespace BTL_Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly BtlWebNcContext _DBbContext;

        public AdminController(BtlWebNcContext dbContext, IWebHostEnvironment webHostEnvironment)
        {
            this._DBbContext = dbContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet] // Define a unique route for the GET action
        [Route("Admin/CreateProduct")]
        public IActionResult CreateProduct()
        {
            return View();
        }

        [HttpPost]
        [Route("Admin/CreateProduct")]
        public IActionResult CreateProduct(Product model, IFormFile productImage)
        {
            try
            {
                if (productImage != null && productImage.Length > 0)
                {
                    var fileName = Path.GetFileName(productImage.FileName);
                    var uploadPath = Path.Combine(_webHostEnvironment.WebRootPath, "Images", fileName);

                    // Check if the directory exists, and if not, create it
                    var directory = Path.GetDirectoryName(uploadPath);
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    using (var stream = new FileStream(uploadPath, FileMode.Create))
                    {
                        productImage.CopyTo(stream);
                    }

                    var newProduct = new Product
                    {
                        ProductName = model.ProductName,
                        ProductImg = fileName,
                        ProductDes = model.ProductDes,
                        ProductPrice = model.ProductPrice
                    };

                    _DBbContext.Set<Product>().Add(newProduct);
                    _DBbContext.SaveChanges();

                    TempData["SuccessMessage"] = "Product created successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid image. Please select a valid image file.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while saving the product information. Please try again later. " + ex.Message;
                Console.WriteLine(ex);
            }

            // Always return the CreateProduct view, whether the form submission was successful or not
            return RedirectToAction("Products", "Admin");

        }


        public IActionResult Products()
        {
            var products = this._DBbContext.Products.ToList();
            return View(products);
        }

        public IActionResult Customes()
        {
            var Users = this._DBbContext.Users.ToList();
            return View(Users);
        }


        public class OrderProductDTO
        {
            public int OrderId { get; set; }
            public string ProductName { get; set; }
            public string ProductImg { get; set; }
            public int ProductPrice { get; set; }
            public string ProductDes { get; set; }
            public int SoLuong { get; set; }
            // Add any other properties you need
        }

        [HttpGet]
        public IActionResult CustomesOrrder()
        {
            var userId = Request.Cookies["id"];

            // Check if userId is null or empty
            if (string.IsNullOrEmpty(userId))
            {
                // Handle the case where userId is empty
                // For example, you might want to redirect to a login page
                return RedirectToAction("Login");
            }

            if (int.TryParse(userId, out int intValue))
            {
                var completedOrdersWithProducts = this._DBbContext.Orders
                    .Where(order => order.Status == "Completed")
                    .Join(
                        this._DBbContext.Products,
                        order => order.ProductId,
                        product => product.ProductId,
                        (order, product) => new BTL_Web.Controllers.AdminController.OrderProductDTO
                        {
                            OrderId = order.OrderId,
                            ProductName = product.ProductName,
                            ProductDes = product.ProductDes,
                            ProductImg = product.ProductImg,
                            SoLuong = (int)order.SoLuong,
                            ProductPrice = (int)product.ProductPrice,
                            // Add other properties you may need
                        })
                    .ToList();

                // Pass the data to the view
                return View(completedOrdersWithProducts);
            }

            // Handle the case where userId is not a valid integer
            // For example, you might want to redirect to an error page
            return RedirectToAction("Error");
        }



        public IActionResult CartDone()
        {
            var userId = Request.Cookies["id"];
            if (userId == "")
            {

            }
            if (int.TryParse(userId, out int intValue))
            {
                var ordersWithProducts = this._DBbContext.Orders
                    .Where(p => p.Status == "Đã đóng hàng")
                    .Join(
                        this._DBbContext.Products,
                        order => order.ProductId,
                        product => product.ProductId,
                        (order, product) => new BTL_Web.Controllers.AdminController.OrderProductDTO
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


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
