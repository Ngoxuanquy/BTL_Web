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
    public IActionResult DangKy(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            TempData["ErrorMessage"] = "Vui lòng nhập tên người dùng và mật khẩu.";
            return RedirectToAction("Index", "DangKy");
        }

        // Check if the username is already registered
        var existingUser = _DBbContext.Users.FirstOrDefault(x => x.Email == username);
        if (existingUser != null)
        {
            TempData["ErrorMessage"] = "Tài khoản đã được đăng ký.";
            return RedirectToAction("Index", "DangKy");
        }

        // Create a new user
        var newUser = new User
        {
            Email = username,
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
