using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using System.Net.Http;
using System;
namespace BTL_Web.Controllers
{
    // [ApiController]
    // [Route("[Controller]")]
    public class ThiController : Controller
    {
        private readonly BtlWebNcContext _DbContext;

        public ThiController(BtlWebNcContext _dbContext)
        {
            this._DbContext = _dbContext;
        }

        [HttpPost]
        // [Route("CreateThi")]
        public JsonResult CreateThi([FromBody] Thi value)
        {
            // Create a new order
            try
            {
                Console.WriteLine(" aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");

                Console.WriteLine(value);

                var newOrder = new Thi
                {
                    // Set other properties as 
                    TenTs = value.TenTs, // Set the UserId to the parsed user ID
                    KetQua = value.KetQua, // Corrected typo "Adrees" to "Address"
                };

                _DbContext.This.Add(newOrder); // Add the new order to the Orders DbSet
                _DbContext.SaveChanges(); // Save changes to the database context


                // Optionally, you can redirect to the appropriate action or view after the order is created
                return Json(new { succ = value });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi lưu đơn đặt hàng. Vui lòng thử lại sau. " + ex.Message;
                // Log the exception for further investigation
                // _logger.LogError(ex, "An error occurred while saving the order data.");
                Console.WriteLine(ex);
                return Json(new { succ = false });

            }

        }


        [HttpPost]
        // [Route("CreateThi")]
        public JsonResult DeleteThi([FromBody] Thi value)
        {
            // Create a new order
            try
            {
                var cartItems = this._DbContext.This.Where(row => row.MaTs == value.MaTs).ToList();

                // Kiểm tra nếu có mục nào trong giỏ hàng thì mới tiến hành xóa
                if (cartItems.Count > 0)
                {
                    // Xóa tất cả các mục trong giỏ hàng có ProductId trùng với id
                    this._DbContext.This.RemoveRange(cartItems);

                    // Lưu các thay đổi vào cơ sở dữ liệu
                    this._DbContext.SaveChanges();
                }


                // Optionally, you can redirect to the appropriate action or view after the order is created
                return Json(new { succ = value });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi lưu đơn đặt hàng. Vui lòng thử lại sau. " + ex.Message;
                // Log the exception for further investigation
                // _logger.LogError(ex, "An error occurred while saving the order data.");
                Console.WriteLine(ex);
                return Json(new { succ = false });

            }

        }



        [HttpPost]
        public IActionResult UpdateThi([FromBody] Thi thivalue)
        {
            try
            {
                var existingProduct = this._DbContext.This.FirstOrDefault(p => p.MaTs == thivalue.MaTs);

                if (existingProduct != null)
                {
                    existingProduct.TenTs = thivalue.TenTs;
                    existingProduct.KetQua = thivalue.KetQua;


                    // Save changes to the database
                    this._DbContext.SaveChanges();

                    return Ok(new { success = true, message = "Product updated successfully", updatedData = existingProduct });
                }
                else
                {
                    return NotFound(new { success = false, message = "Product not found", updatedData = existingProduct });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Error updating product", error = ex.Message });
            }
        }

        // [HttpGet("GetAll")]
        public IActionResult Index()
        {
            var ThiValue = this._DbContext.This.ToList();
            return View(ThiValue);
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
