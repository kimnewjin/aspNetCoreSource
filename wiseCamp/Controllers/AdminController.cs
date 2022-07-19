using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace wiseCamp.Controllers
{
    public class AdminController : Controller
    {
        const string sessionAdminName = "_adminName";
        const string sessionAdminID = "_adminID";
        const string sessionAdminCategory = "";
        const string sessionCompanyID = "";

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");

            HttpContext.Session.SetString(sessionAdminName, "");
            HttpContext.Session.SetString(sessionAdminID, "");
            HttpContext.Session.SetString(sessionAdminCategory, "");
            HttpContext.Session.SetString(sessionCompanyID, "");
            // HttpContext.User.Identity.IsAuthenticated = false;

            //return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.Authentication.SignOutAsync("Cookies");

            HttpContext.Session.SetString(sessionAdminName, null);
            HttpContext.Session.SetString(sessionAdminID, null);
            HttpContext.Session.SetString(sessionAdminCategory, null);
            HttpContext.Session.SetString(sessionCompanyID, null);
            // HttpContext.User.Identity.IsAuthenticated = false;

            return RedirectToAction("Index", "Home");

        }
    }
}
