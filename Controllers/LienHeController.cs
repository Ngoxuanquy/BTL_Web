using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;

namespace BTL_Web.Controllers
{
    public class LienHeController : Controller
    {
        private readonly BtlWebNcContext _DbContext;

        public LienHeController(BtlWebNcContext _dbContext)
        {
            this._DbContext = _dbContext;
        }

    // [HttpGet("GetAll")]
        public IActionResult Index()
        {
            var users = this._DbContext.Users.ToList();
            return View(users);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
