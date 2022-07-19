using Dapper;
using System;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using wiseCamp.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;



namespace wiseCamp.Controllers
{
    public class HomeController : Controller
    {

        private IHomeRepository _repository;
        private IConfiguration _config;
        private SqlConnection con;

        /*세션명 지정*/

        const string sessionAdminName = "_adminName";
        const string sessionAdminID = "_adminID";
        const string sessionAdminCategory = "" ;
        const string sessionCompanyID = "" ;





        public  HomeController(IConfiguration config,IHomeRepository repository)
        {
            _config = config;
            _repository = repository;
            con = new SqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]

        public async Task <IActionResult> Index(HomeViewModel model,string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                string adminName = _repository.IsCorrectUser(model.CompanyID, model.UserID, model.PassWD, model.ManagerCate);

               
                if (adminName != null){
                    var claims = new List<Claim>()
                    {
                        new Claim("userID", model.UserID),
                        new Claim(ClaimTypes.Role, "Users")
                    };
                    

                    var userIP = HttpContext.Connection.RemoteIpAddress.ToString();
                   // +@"   @memID, @mName, @mCate, @mCompanyId, @userIP, getDate()"
                    string Query = @" Insert Into dbb2b.dbo.tblSoobakcb2bAdminLoginHistory   ( "
                                 + @"   memID,  mName,  mCate,  mCompanyID, userIP, regDate "
                                 + @" ) values (    "
                                 + @"   '" + model.UserID + "','"+ adminName + "', '"+ model.ManagerCate + "', '"+ model.CompanyID + "', '"+ userIP + "', getDate()"
                                 + @")";
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;
                    cmd.CommandText = Query;
                    cmd.CommandType = CommandType.Text;

                 /*   cmd.Parameters.AddWithValue("@memID", model.UserID);
                    cmd.Parameters.AddWithValue("@mName", adminName);
                    cmd.Parameters.AddWithValue("@mCate", model.ManagerCate);
                    cmd.Parameters.AddWithValue("@mCompanyId", model.CompanyID);
                    cmd.Parameters.AddWithValue("@userIP", userIP);*/

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    HttpContext.Session.SetString(sessionAdminName, adminName);
                    HttpContext.Session.SetString(sessionAdminID, model.UserID);
                    HttpContext.Session.SetString(sessionAdminCategory, model.ManagerCate);
                    HttpContext.Session.SetString(sessionCompanyID, model.CompanyID);


                    var ci = new ClaimsIdentity(claims, model.PassWD);
                    await HttpContext.Authentication.SignInAsync("Cookies", new ClaimsPrincipal(ci));
                    return LocalRedirect("/Member/Index");
                }
            }

            return View(model);
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

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
