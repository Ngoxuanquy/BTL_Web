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
