using wiseCamp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text;
using System.Text.Encodings.Web;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace wiseCamp.Controllers
{
    public class MemberController : Controller
    {
        private IHostingEnvironment _environment;
        private IMemberRepository _repository;

        const string sessionAdminName = "_adminName";
        const string sessionAdminID = "_adminID";
        const string sessionAdminCategory = "";
        const string sessionCompanyID = "";


        public MemberController(IHostingEnvironment environment, IMemberRepository repository)
        {
            _environment = environment;
            _repository = repository;
        }
        public string userIP { get; set; }
        public bool searchMode { get; set; } = false;
        public string searchModeflag { get; set; } = "";
        public string profInfo { get; set; }
        public string profInfoText { get; set; }
        public string memInfoCate { get; set; }
        public string memInfoText { get; set; }
        public string memStatus { get; set; }
        public string enterRoot { get; set; }
        public string memberStartDate { get; set; }
        public string memberEndDate { get; set; }

        public int PageIndex { get; set; } = 1;
        public int TotalRecordCount { get; set; } = 0;

        public int UpdateSubProfInfoFlag { get; set; } = 0;

       

        // GET: /<controller>/

        public IActionResult Excel(string searchModeflag, string profInfo, string profInfoText, string memInfoCate, string memInfoText, string memStatus, string enterRoot, string memberStartDate, string memberEndDate, string OrderByFlag)
        {
            IEnumerable<MemberExcel> memberexcel;
          

         

          //  string ExcelFileName = "와";
          //  byte[] byteExcel = Encoding.GetEncoding(51949).GetBytes(ExcelFileName);
          //  string ExcelName = Encoding.GetEncoding(51949).GetString(byteExcel);

           
           
            HttpContext.Response.Headers.Add("content-Type", "application/vnd.ms-excel");
            HttpContext.Response.Headers.Add("Content-Disposition","attachment;filename="+ System.Net.WebUtility.UrlEncode("와이즈캠프회원현황.xls")  );
            HttpContext.Response.Headers.Add("Charset", "euc-kr");
           

            memberexcel = _repository.GetExcel(10001, profInfo, profInfoText, memInfoCate, memInfoText, memStatus, enterRoot, memberStartDate, memberEndDate, OrderByFlag);            

            return View(memberexcel);
        }
        public IActionResult Index(string searchModeflag, string profInfo, string profInfoText, string memInfoCate, string memInfoText, string memStatus, string enterRoot, string memberStartDate, string memberEndDate, string OrderByFlag)
        {
           
            if(searchModeflag == "true")
            {
                searchMode = true;
            }
           
            if (!string.IsNullOrEmpty(Request.Query["gotoPage"].ToString()))
            {
                PageIndex = Convert.ToInt32(Request.Query["gotoPage"]);
            }
           

            IEnumerable<Member> member;
            int pageSize =30;
            TotalRecordCount = _repository.GetCountAll(10001, profInfo, profInfoText, memInfoCate, memInfoText,  memStatus,  enterRoot,  memberStartDate,  memberEndDate);
            
            ViewBag.pageIndex = PageIndex;
            ViewBag.TotalRecord = TotalRecordCount;
            ViewBag.searchMode = searchMode;
            ViewBag.profInfo = profInfo;
            ViewBag.profInfoText = profInfoText;
            ViewBag.memInfoCate = memInfoCate;
            ViewBag.memInfoText = memInfoText;
            ViewBag.memStatus = memStatus;
            ViewBag.enterRoot = enterRoot;
            ViewBag.memberStartDate = memberStartDate;
            ViewBag.memberEndDate = memberEndDate;
            ViewBag.minusVal = 1;
            ViewBag.userIP = HttpContext.Connection.RemoteIpAddress.ToString();
            ViewBag.SessionAdminName = HttpContext.Session.GetString(sessionAdminName);

            ViewBag.pNo = TotalRecordCount;

            if (PageIndex != 1)
            {
                ViewBag.pNo = ViewBag.pNo - ((PageIndex - 1) * pageSize);
            }
            member = _repository.GetAll(PageIndex, pageSize, 10001, profInfo, profInfoText, memInfoCate, memInfoText, memStatus, enterRoot, memberStartDate, memberEndDate, OrderByFlag, ViewBag.pNo);

            return View(member);

        
        }

        public IActionResult editsubProfID(EidtSubPorfID model)
        {
            int memIdx = Convert.ToInt32(Request.Query["memIdx"]);
     

            ViewBag.memIdx = memIdx;
            ViewBag.mem_ID = _repository.GetsubProfIDInfo(memIdx).Mem_ID;
            ViewBag.m_Name = _repository.GetsubProfIDInfo(memIdx).M_Name;
            ViewBag.subProfID = _repository.GetsubProfIDInfo(memIdx).SubProfID;
            ViewBag.subProfName = _repository.GetsubProfIDInfo(memIdx).SubProfName; 



            return View();
        }

        public IActionResult goUpdateSubProfInfo(int memIdx, string mem_ID, string subProfID, string subProfName)
        {

            UpdateSubProfInfoFlag = _repository.goUpdateSubProfInfo(memIdx, mem_ID, subProfID, subProfName);
            ViewBag.UpdateSubProfInfoFlag = UpdateSubProfInfoFlag;
           

            return View();
        }

    }
}
