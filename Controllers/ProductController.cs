using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System;

namespace BTL_Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : Controller
    {
        private readonly BtlWebNcContext _DbContext;
        private readonly ILogger<BtlWebNcContext> _logger;

        public ProductController(BtlWebNcContext _dbContext, ILogger<ProductController> logger)
        {
            this._DbContext = _dbContext;
            _logger = _logger;

        }

        [HttpGet("GetAll")]
        public IActionResult Index()
        {
            var users = this._DbContext.Products.ToList();
            return View(users);
        }


        [HttpGet]
        public ActionResult ProductDetail(int id)
        {
            var product = this._DbContext.Products.FirstOrDefault(p => p.ProductId == id);
            if (product == null)
            {
                return NotFound(); // Or any other action you want to take if the product doesn't exist
            }
            return View(product);
            // return RedirectToAction("Index", "Login");
            //      TempData["ProductData"] = product; // Storing the product data in TempData
            // return RedirectToAction("ProductDetail", "Product");
        }



        [HttpPost]
        [Route("UpdateProduct")]
        public IActionResult UpdateProduct([FromBody] Product productUpdateModel)
        {
            try
            {
                var existingProduct = this._DbContext.Products.FirstOrDefault(p => p.ProductId == productUpdateModel.ProductId);

                if (existingProduct != null)
                {
                    existingProduct.ProductName = productUpdateModel.ProductName;
                    existingProduct.ProductDes = productUpdateModel.ProductDes;
                    existingProduct.ProductImg = productUpdateModel.ProductImg;
                    existingProduct.ProductPrice = productUpdateModel.ProductPrice;


                    // Save changes to the database
                    this._DbContext.SaveChanges();

                    return Ok(new { success = true, message = "Product updated successfully", updatedData = existingProduct });
                }
                else
                {
                    return NotFound(new { success = false, message = "Product not found" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error updating product", error = ex.Message });
            }
        }




        [HttpPost]
        // [ValidateAntiForgeryToken] // Use anti-forgery token for security
        public JsonResult DeleteCart(int id)
        {
            Console.WriteLine($"Attempting to delete products with ProductId {id}");

            try
            {
                // Find all cart items with the specified ProductId
                using (var transaction = this._DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var cartItems = this._DbContext.Products.Where(p => p.ProductId == id).ToList();

                        Console.WriteLine($"Found {cartItems.Count} products with ProductId {id}");

                        if (cartItems.Count > 0)
                        {
                            // Remove all found cart items
                            this._DbContext.Products.RemoveRange(cartItems);

                            Console.WriteLine($"Successfully deleted {cartItems.Count} products with ProductId {id}");
                            // Save changes to the database
                            this._DbContext.SaveChanges();

                            transaction.Commit(); // Commit the transaction if everything is successful

                            return Json(new { message = "Products successfully deleted." });
                        }
                        else
                        {
                            Console.WriteLine($"No products found with ProductId {id}");
                            return Json(new { message = "No products found with the specified ProductId." });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception, you can log it or take appropriate action based on your application's requirements.
                        Console.WriteLine($"An error occurred while fetching or deleting cart items: {ex.Message}");

                        transaction.Rollback(); // Rollback the transaction in case of an error
                        return Json(new { message = "Error occurred while processing the request." + ex });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting products with ProductId {id}: {ex.Message}");
                return Json(500, "Internal Server Error"); // Or any other appropriate error response
            }
        }



        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




    }
}
