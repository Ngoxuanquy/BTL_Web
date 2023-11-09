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
                    return RedirectToAction("Index", "Login");
                }

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
