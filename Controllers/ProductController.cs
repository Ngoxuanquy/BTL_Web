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

        [HttpPost] // Add an appropriate route here
        [Route("Filter")]
        public JsonResult Filter([FromBody] Product id)
        {

            var filteredProducts = this._DbContext.Products
                                                       .Where(p => p.ProductPrice > id.ProductPrice)
                                                       .ToList();
            return Json(new { succ = filteredProducts });
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
        [Route("DeleteCart")]
        public IActionResult DeleteCart([FromBody] Product id)
        {
            try
            {

                using (var transaction = this._DbContext.Database.BeginTransaction())
                {
                    try
                    {
                        var cartItems = this._DbContext.Products.Where(p => p.ProductId == id.ProductId).ToList();

                        if (cartItems.Any())
                        {
                            this._DbContext.Products.RemoveRange(cartItems);
                            this._DbContext.SaveChanges();
                            transaction.Commit();

                            Console.WriteLine(cartItems);


                            return Json(new { message = "Products successfully deleted." });
                        }
                        else
                        {
                            return Json(new { message = "No products found with the specified ProductId.", data = id });
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception
                        Console.WriteLine($"An error occurred while fetching or deleting cart items: {ex.Message}");
                        transaction.Rollback();
                        throw; // Let the exception propagate up
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error deleting products with ProductId {id}: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }




        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }




    }
}
