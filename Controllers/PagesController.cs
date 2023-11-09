using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BTL_Web.Models;

namespace BTL_Web.Controllers
{
    public class PagesController : Controller
    {
        private readonly BtlWebNcContext _DbContext;

        public PagesController(BtlWebNcContext _dbContext)
        {
            this._DbContext = _dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
