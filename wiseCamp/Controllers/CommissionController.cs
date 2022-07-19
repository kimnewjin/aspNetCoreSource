using wiseCamp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace wiseCamp.Controllers
{
    public class CommissionController : Controller
    {
        private IHostingEnvironment _environment;
        private ICommissionRepository _repository;

        const string sessionAdminName = "_adminName";
        const string sessionAdminID = "_adminID";
        const string sessionAdminCategory = "";
        const string sessionCompanyID = "";


        public CommissionController(IHostingEnvironment environment, ICommissionRepository repository)
        {
            _environment = environment;
            _repository = repository;
        }

        public IActionResult Excel(string startDate, string endDate, string productInfo, string feeStatus)
        {
            IEnumerable<CommissionExcel> CommissionExcel;




            //  string ExcelFileName = "와";
            //  byte[] byteExcel = Encoding.GetEncoding(51949).GetBytes(ExcelFileName);
            //  string ExcelName = Encoding.GetEncoding(51949).GetString(byteExcel);



            HttpContext.Response.Headers.Add("content-Type", "application/vnd.ms-excel");
            HttpContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + System.Net.WebUtility.UrlEncode("와이즈캠프수수료정산현황.xls"));
            HttpContext.Response.Headers.Add("Charset", "euc-kr");


            CommissionExcel = _repository.GetCommisionListExcel(10001, startDate, endDate, productInfo, feeStatus);

            return View(CommissionExcel);
        }

        // GET: /<controller>/
        public IActionResult Index(string startDate, string endDate, string productInfo, string feeStatus)
        {
          

            IEnumerable<Commission> Commission;

            List<SalesProductItem> SalesProductItem = new List<SalesProductItem>();

            SalesProductItem = _repository.GetProductList(10001);

            SalesProductItem.Insert(0, new SalesProductItem { lp_idx = " ", lp_title = "상품정보보기" });

            Commission = _repository.GetCommisionList(10001, startDate, endDate, productInfo, feeStatus);
            ViewBag.userIP = HttpContext.Connection.RemoteIpAddress.ToString();
            ViewBag.SessionAdminName = HttpContext.Session.GetString(sessionAdminName);
            ViewBag.SessioncompanyID = HttpContext.Session.GetString(sessionCompanyID);
            ViewBag.FeeStatus = feeStatus;
            ViewBag.lpidx = productInfo;
            ViewBag.SalesProductItemList = SalesProductItem;

            ViewBag.startDate = startDate;
            ViewBag.endDate = endDate;

            return View(Commission);
        }
    }
}
