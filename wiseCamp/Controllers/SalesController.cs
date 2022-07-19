 using Dapper;
using wiseCamp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
    public class SalesController : Controller
    {
        private IHostingEnvironment _environment;
        private ISalesRepository _repository;
        private readonly DatabaseContext _context;

        const string sessionAdminName = "_adminName";
        const string sessionAdminID = "_adminID";
        const string sessionAdminCategory = "";
        const string sessionCompanyID = "";


        public SalesController(IHostingEnvironment environment, ISalesRepository repository, DatabaseContext context)
        {
            _environment = environment;
            _repository = repository;
            _context = context;
        }
        public int PageIndex { get; set; } = 1;
        public int TotalRecordCount { get; set; } = 0;

        // GET: /<controller>/

        public IActionResult Excel(string profInfo, string profInfoText, string productInfo, string memInfoCate, string memInfoText, string orderStatus, string dateCategory, string startDate, string endDate, string orderByflag)
        {
            IEnumerable<SalesExcel> SalesExcel;
            HttpContext.Response.Headers.Add("content-Type", "application/vnd.ms-excel");
            HttpContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + System.Net.WebUtility.UrlEncode("와이즈캠프결제현황.xls"));
            SalesExcel = _repository.GetSalesExcel(10001, profInfo, profInfoText, productInfo, memInfoCate, memInfoText, orderStatus, dateCategory, startDate, endDate, orderByflag);

            return View(SalesExcel);
        }

        public IActionResult Index(string profInfo, string profInfoText, string productInfo, string findItemType1, string findItemType2, string memInfoCate, string memInfoText, string orderStatus, string dateCategory, string startDate, string endDate, string orderByflag)
        {
            if (!string.IsNullOrEmpty(Request.Query["gotoPage"].ToString()))
            {
                PageIndex = Convert.ToInt32(Request.Query["gotoPage"]);
            }

            IEnumerable<Sales> sales;

            List<SalesProductItem> SalesProductItem = new List<SalesProductItem>();

            SalesProductItem = _repository.GetProductList(10001);

            SalesProductItem.Insert(0, new SalesProductItem { lp_idx = " ", lp_title = "상품정보보기" });
            int pageSize = 15;
            TotalRecordCount = _repository.GetOrderCount(10001, profInfo, profInfoText, productInfo, memInfoCate, memInfoText, orderStatus, dateCategory, startDate, endDate);

            //string ttt = _repository.GetOrderCount(10001, profInfo, profInfoText, productInfo, memInfoCate, memInfoText, orderStatus, dateCategory, startDate, endDate);
            //ViewBag.ttt = ttt;
            ViewBag.TotalRecord = TotalRecordCount;
            ViewBag.pageSize = pageSize;
            ViewBag.userIP = HttpContext.Connection.RemoteIpAddress.ToString();
            ViewBag.SessionAdminName = HttpContext.Session.GetString(sessionAdminName);
            ViewBag.SalesProductItemList = SalesProductItem;

            ViewBag.profInfo        = profInfo;
            ViewBag.profInfoText    = profInfoText;
            ViewBag.productInfo     = productInfo;
            ViewBag.memInfoCate     = memInfoCate;
            ViewBag.memInfoText     = memInfoText;
            ViewBag.orderStatus     = orderStatus;
            ViewBag.dateCategory    = dateCategory;                   
            ViewBag.startDate       = startDate;
            ViewBag.endDate         = endDate;
            ViewBag.findItemType1 = findItemType1;
            ViewBag.findItemType2 = findItemType2;

            ViewBag.pNo = TotalRecordCount;

            if (PageIndex != 1)
            {
                ViewBag.pNo = ViewBag.pNo - ((PageIndex - 1) * pageSize);
            }


            sales = _repository.GetOrderList(PageIndex, pageSize, 10001, profInfo, profInfoText, productInfo, memInfoCate, memInfoText, orderStatus, dateCategory, startDate, endDate, ViewBag.pNo, orderByflag);

            //ViewBag.SList = sales;
            return View(sales);
        }
    }
}

