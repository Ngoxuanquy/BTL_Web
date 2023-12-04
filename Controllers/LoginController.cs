using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;
using Microsoft.AspNetCore.Http;

namespace BTL_Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly BtlWebNcContext _DBbContext;

        public LoginController(BtlWebNcContext dbContext)
        {
            this._DBbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Check if the counter is already stored in session, initialize to 0 if not
            int loginAttempts = Request.Cookies.ContainsKey("LoginAttempts")
                ? int.Parse(Request.Cookies["LoginAttempts"])
                : 0;

            if (loginAttempts >= 1)
            {
                TempData["ErrorMessage"] = "Bạn đã nhập sai mật khẩu quá 3 lần. Vui lòng thử lại sau 1 phút";
                return RedirectToAction("Index", "Login");
            }

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập tên người dùng và mật khẩu.";
                return RedirectToAction("Index", "Login");
            }
            else
            {
                var existingUser = this._DBbContext.Users.FirstOrDefault(x => x.Email == username && x.Password == password);

                if (existingUser == null)
                {
                    TempData["ErrorMessage"] = "Tài khoản không tồn tại.";

                    // Increment the login attempt counter
                    loginAttempts++;

                    Response.Cookies.Append("LoginAttempts", loginAttempts.ToString(), new CookieOptions
                    {
                        Expires = DateTime.Now.AddMinutes(1) // Set the cookie to expire in 1 day
                    });

                    return RedirectToAction("Index", "Login");
                }

                // Reset the login attempt counter upon successful login
                Response.Cookies.Append("LoginAttempts", "0", new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                });

                // Tạo một cookie mới
                Response.Cookies.Append("usename", username, new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                });

                Response.Cookies.Append("id", existingUser.UserId.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                });

                if (username == "admin")
                {
                    return RedirectToAction("Index", "Admin");
                }

                Console.WriteLine(existingUser.UserId);

                int totalProductQuantity = this._DBbContext.Orders
                    .Where(o => o.Status == "Đặt hàng" && o.UserId == existingUser.UserId && o.SoLuong.HasValue)
                    .Sum(o => o.SoLuong.Value);

                Console.WriteLine("totalProductQuantity: " + totalProductQuantity);

                Response.Cookies.Append("soluong", totalProductQuantity.ToString(), new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(1) // Set the cookie to expire in 1 day
                });


                return RedirectToAction("Index", "Home");
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
