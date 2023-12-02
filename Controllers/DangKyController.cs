using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;

namespace BTL_Web.Controllers;

public class DangKyController : Controller
{

    private readonly BtlWebNcContext _DBbContext;

    public DangKyController(BtlWebNcContext dbContext)
    {
        _DBbContext = dbContext;

    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public IActionResult DangKy(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ((List<string>)TempData["ErrorMessage"]).Add("Vui lòng nhập tên người dùng và mật khẩu.");
            // TempData["ErrorMessage"] = email;
            // TempData["ErrorMessage"] = password;


            return RedirectToAction("Index", "DangKy");
        }

        // Check if the email is already registered
        var existingUser = _DBbContext.Users.FirstOrDefault(x => x.Email == email);
        if (existingUser != null)
        {
            TempData["ErrorMessage"] = "Tài khoản đã được đăng ký.";
            return RedirectToAction("Index", "DangKy");
        }

        if (IsGmailAddress(email))
        {
            var newUser = new User
            {
                Email = email,
                Password = password
            };

            try
            {
                _DBbContext.Set<User>().Add(newUser); // Use Set<User>() to explicitly set the entity type
                _DBbContext.SaveChanges();
                return RedirectToAction("", "Login");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi lưu thông tin người dùng. Vui lòng thử lại sau." + ex;
                // Log the exception for further investigation
                // _logger.LogError(ex, "An error occurred while saving user data.");
                Console.WriteLine(ex);
                return RedirectToAction("Index", "DangKy");
            }
        }
        else
        {
            TempData["ErrorMessage"] = "Vui lòng nhập email có đuôi @gmail.com";

        }
        // Create a new user

        return RedirectToAction("Index", "DangKy");


    }

    bool IsGmailAddress(string email)
    {
        return email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase);
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
