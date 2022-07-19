using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;

namespace wiseCamp.Models
{
    public class SalesRepository : ISalesRepository
    {
        private IConfiguration _config;
        private SqlConnection con;
        private ILogger<SalesRepository> _logger;


        public SalesRepository(IConfiguration config, ILogger<SalesRepository> logger)
        {
            _config = config;
            con = new SqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            _logger = logger;
        }

        public int GetOrderCount(int companyID, string profInfo, string profInfoText, string productInfo, string memInfoCate, string memInfoText, string orderStatus, string dateCategory, string startDate, string endDate)
        {
            string subQuery = "";

            if (profInfo == null) { profInfo = ""; }
            if (profInfoText == null) { profInfoText = ""; }
            if (productInfo == null) { productInfo = ""; }
            if (memInfoCate == null) { memInfoCate = ""; }
            if (memInfoText == null) { memInfoText = ""; }
            if (orderStatus == null) { orderStatus = ""; }
            if (dateCategory == null) { dateCategory = ""; }
            if (startDate == null) { startDate = ""; }
            if (endDate == null) { endDate = ""; }

            if (profInfo != "" && profInfoText != "")
            {
              subQuery = subQuery + " And total.subprofID like @profInfoText ";
            }

            if(productInfo != "")
            {
                subQuery = subQuery + " And total.lp_idx = @productInfo";
            }


            if (memInfoCate != "" && memInfoText != "")
            {
                switch (memInfoCate)
                {
                    case "total.mem_Id":
                        subQuery = subQuery + " And total.mem_Id like @memInfoText ";
                        break;
                    case "total.m_Name":
                        subQuery = subQuery + " And total.m_Name like @memInfoText ";
                        break;
                    case "total.m_HP":
                        subQuery = subQuery + " And total.m_HP like @memInfoText ";
                        break;
                    case "total.familysiteMemID":
                        subQuery = subQuery + " And total.familysiteMemID like @memInfoText ";
                        break;
                    case "total.familysiteMemName":
                        subQuery = subQuery + " And total.familysiteMemName like @memInfoText ";
                        break;
                }
            }

            if (orderStatus != "")
            {
               subQuery = subQuery + "	And total.orderStatus = @orderStatus	";               
            }


            if (dateCategory != "" && startDate != "" && endDate != "")
            {
                switch (dateCategory)
                {
                    case "R":
                        subQuery = subQuery + " And Convert(Date,total.reg_date) between @startDate And @endDate    ";
                        break;
                    case "O":
                        subQuery = subQuery + " And Convert(Date,total.authdate) between @startDate  And @endDate    ";
                        subQuery = subQuery + " And total.orderStatus = '1' ";
                        break;
                    case "C":
                        subQuery = subQuery + " And Convert(Date,total.authdate)  between @startDate  And @endDate     ";
                        subQuery = subQuery + " And total.orderStatus = '2' ";
                        break;
                }
            }

            String Query = @"    Select Count(1) From ( "
                            + @"	    Select A.* , Od1.Status As orderStatus  "
                            + @"            , mb.reg_date, bm.subProfID, mb.mem_ID, od1.authDate    "
                            + @"            , mb.m_Name,mb.m_Hp, bm.familysiteMemID , bm.familysiteMemName   "
                            + @"        From dbB2B.dbo.tblSoobakcB2BSalesItemList  as A	"
                            + @"  	        Inner Join dbSoobakc.dbo.tblOrderdetailLevel1 As Od1  on A.lp_Idx = Od1.lp_Idx	"
                            + @"		    Inner Join soobakc_test.dbo.tbl_Member_basic As MB on OD1.userID = mb.mem_ID	"
                            + @"           Inner Join soobakc_test.dbo.tbl_lecture_Pack As Lp on OD1.lp_idx = LP.lp_Idx "
                            + @"		    left outer join (	"
                            + @"	    	    Select * from dbb2b.dbo.tblSoobakcB2BmemInfo Where memStatus='1019' And companyID =@companyID"
                            + @"		    ) As Bm on mb.mem_ID = bm.mem_ID	"
                            + @"	    Where A.isDel='N' and A.companyID =@companyID "
                            + @"    ) As total Where 1 = 1  " + subQuery ;

            //return Query;
            var Parameters = new DynamicParameters(new {  companyID = companyID, profInfo = profInfo, profInfoText = '%'+ profInfoText+'%', productInfo = productInfo, memInfoCate = memInfoCate, memInfoText = '%'+ memInfoText+'%', orderStatus = orderStatus , dateCategory = dateCategory, startDate= startDate, endDate = endDate });
            return con.Query<int>(Query, Parameters, commandType: CommandType.Text).SingleOrDefault();


        }

        public List<SalesExcel> GetSalesExcel(int companyID, string profInfo, string profInfoText, string productInfo, string memInfoCate, string memInfoText, string orderStatus, string dateCategory, string startDate, string endDate, string orderByflag)
        {
            string subQuery = "";

            if (profInfo == null) { profInfo = ""; }
            if (profInfoText == null) { profInfoText = ""; }
            if (productInfo == null) { productInfo = ""; }
            if (memInfoCate == null) { memInfoCate = ""; }
            if (memInfoText == null) { memInfoText = ""; }
            if (orderStatus == null) { orderStatus = ""; }
            if (dateCategory == null) { dateCategory = ""; }
            if (startDate == null) { startDate = ""; }
            if (endDate == null) { endDate = ""; }


            if (profInfo != "" && profInfoText != "")
            {
                subQuery = subQuery + " And total.subprofID like @profInfoText ";
            }

            if (productInfo != "")
            {
                subQuery = subQuery + " And total.lp_idx = @productInfo";
            }


            if (memInfoCate != "" && memInfoText != "")
            {
                switch (memInfoCate)
                {
                    case "total.mem_Id":
                        subQuery = subQuery + " And total.mem_Id like @memInfoText ";
                        break;
                    case "total.m_Name":
                        subQuery = subQuery + " And total.m_Name like @memInfoText ";
                        break;
                    case "total.m_HP":
                        subQuery = subQuery + " And total.m_HP like @memInfoText ";
                        break;
                    case "total.familysiteMemID":
                        subQuery = subQuery + " And total.familysiteMemID like @memInfoText ";
                        break;
                    case "total.familysiteMemName":
                        subQuery = subQuery + " And total.familysiteMemName like @memInfoText ";
                        break;
                }
            }

            if (orderStatus != "")
            {
                subQuery = subQuery + "	And total.orderStatus = @orderStatus	";
            }


            if (dateCategory != "" && startDate != "" && endDate != "")
            {
                switch (dateCategory)
                {
                    case "R":
                        subQuery = subQuery + " And Convert(Date,total.reg_date) between @startDate And @endDate    ";
                        break;
                    case "O":
                        subQuery = subQuery + " And Convert(Date,total.authdate) between @startDate  And @endDate    ";
                        subQuery = subQuery + " And total.orderStatus = '1' ";
                        break;
                    case "C":
                        subQuery = subQuery + " And Convert(Date,total.authdate)  between @startDate  And @endDate     ";
                        subQuery = subQuery + " And total.orderStatus = '2' ";
                        break;
                }
            }
           

            string OrderByQuery = "";

            switch (orderByflag)
            {
                case "A":
                    OrderByQuery = "Order by mem_ID Asc, Idx Desc";
                    break;
                case "B":
                    OrderByQuery = "Order by m_Name Asc, Idx Desc";
                    break;
                case "C":
                    OrderByQuery = "Order by reg_Date Asc, Idx Desc";
                    break;
                case "D":
                    OrderByQuery = "Order by m_Hp Asc, Idx Desc";
                    break;
                case "E":
                    OrderByQuery = "Order by subProfID Asc , Idx Desc";
                    break;
                case "F":
                    OrderByQuery = "Order by subProfName Asc, Idx Desc";
                    break;
                case "G":
                    OrderByQuery = "Order By familysitememID Asc, Idx Desc";
                    break;

                default:
                    OrderByQuery = "Order by dt1Seq Desc";
                    break;
            }


            string Query = @"	Select * from (	"
                         + @"  Select row_Number() over(" + OrderByQuery + ")	As rowNum , * From (	"
                         + @"      Select  mb.mem_ID, mb.m_Name, mb.reg_Date, mb.m_Hp, bm.subProfID, bm.subProfName	"
                         + @"           , bm.familysitememID, od1.itemName, od1.payAmount, od1.authDate,od1.Status As orderStatus   "
                         + @"			, bm.Idx , A.lp_idx , Od1.dt1Seq, Lp.isMonthlyProduct "
                         + @"           , (Select top 1 fp_expire_Date From soobakc_test.dbo.tbl_Freepass_His Where fp_mem_ID = OD1.userID And O_ID = OD1.orderID) As fpExpireDate "
                         + @"           , (Select top 1 fp_isDel From soobakc_test.dbo.tbl_Freepass_His Where fp_mem_ID = OD1.userID And O_ID = OD1.orderID) As fpisDel "
                         + @"		From dbB2B.dbo.tblSoobakcB2BSalesItemList  as A		"
                         + @"			Inner Join dbSoobakc.dbo.tblOrderdetailLevel1 As Od1  on A.lp_Idx = Od1.lp_Idx	"
                         + @"			Inner Join soobakc_test.dbo.tbl_Member_basic As MB on OD1.userID = mb.mem_ID		"
                         + @"           Inner Join soobakc_test.dbo.tbl_lecture_Pack As Lp on OD1.lp_idx = LP.lp_Idx "
                         + @"			Left outer join (	"
                         + @"  			Select * from dbb2b.dbo.tblSoobakcB2BmemInfo Where memStatus='1019' And companyID = @companyID  "
                         + @"          ) As Bm on mb.mem_ID = bm.mem_ID	"
                         + @"		Where A.isDel='N' and A.companyID = @companyID	"
                          + @"	)	As total	"
                         + @"	Where 1 = 1	" + subQuery
                         + @"	) as totalList where 1 = 1	"                       
                         + @"    Order By rowNum Asc";


            var Parameters = new DynamicParameters(new {  companyID = companyID,  profInfo = profInfo, profInfoText = '%' + profInfoText + '%', productInfo = productInfo, memInfoCate = memInfoCate, memInfoText = '%' + memInfoText + '%', orderStatus = orderStatus, dateCategory = dateCategory, startDate = startDate, endDate = endDate });
            return con.Query<SalesExcel>(Query, Parameters, commandType: CommandType.Text).ToList();


        }


        public List<Sales> GetOrderList(int gotoPage, int pageSize, int companyID, string profInfo, string profInfoText, string productInfo, string memInfoCate, string memInfoText, string orderStatus, string dateCategory, string startDate, string endDate, int chkNo, string orderByflag)       
        {
            string subQuery = "";

            if (profInfo == null) { profInfo = ""; }
            if (profInfoText == null) { profInfoText = ""; }
            if (productInfo == null) { productInfo = ""; }
            if (memInfoCate == null) { memInfoCate = ""; }
            if (memInfoText == null) { memInfoText = ""; }
            if (orderStatus == null) { orderStatus = ""; }
            if (dateCategory == null) { dateCategory = ""; }
            if (startDate == null) { startDate = ""; }
            if (endDate == null) { endDate = ""; }


            if (profInfo != "" && profInfoText != "")
            {
                subQuery = subQuery + " And total.subprofID like @profInfoText ";
            }

            if (productInfo != "")
            {
                subQuery = subQuery + " And total.lp_idx = @productInfo";
            }


            if (memInfoCate != "" && memInfoText != "")
            {
                switch (memInfoCate)
                {
                    case "total.mem_Id":
                        subQuery = subQuery + " And total.mem_Id like @memInfoText ";
                        break;
                    case "total.m_Name":
                        subQuery = subQuery + " And total.m_Name like @memInfoText ";
                        break;
                    case "total.m_HP":
                        subQuery = subQuery + " And total.m_HP like @memInfoText ";
                        break;
                    case "total.familysiteMemID":
                        subQuery = subQuery + " And total.familysiteMemID like @memInfoText ";
                        break;
                    case "total.familysiteMemName":
                        subQuery = subQuery + " And total.familysiteMemName like @memInfoText ";
                        break;
                }
            }

            if (orderStatus != "")
            {
                subQuery = subQuery + "	And total.orderStatus = @orderStatus	";
            }


            if (dateCategory != "" && startDate != "" && endDate != "")
            {
                switch (dateCategory)
                {
                    case "R":
                        subQuery = subQuery + " And Convert(Date,total.reg_date) between @startDate And @endDate    ";
                        break;
                    case "O":
                        subQuery = subQuery + " And Convert(Date,total.authdate) between @startDate  And @endDate    ";
                        subQuery = subQuery + " And total.orderStatus = '1' ";
                        break;
                    case "C":
                        subQuery = subQuery + " And Convert(Date,total.authdate)  between @startDate  And @endDate     ";
                        subQuery = subQuery + " And total.orderStatus = '2' ";
                        break;
                }
            }
            chkNo = chkNo + 1;

            string OrderByQuery = "";

            switch (orderByflag)
            {
                case "A":
                    OrderByQuery = "Order by mem_ID Asc, Idx Desc";
                    break;
                case "B":
                    OrderByQuery = "Order by m_Name Asc, Idx Desc";
                    break;
                case "C":
                    OrderByQuery = "Order by reg_Date Asc, Idx Desc";
                    break;
                case "D":
                    OrderByQuery = "Order by m_Hp Asc, Idx Desc";
                    break;
                case "E":
                    OrderByQuery = "Order by subProfID Asc , Idx Desc";
                    break;
                case "F":
                    OrderByQuery = "Order by subProfName Asc, Idx Desc";
                    break;
                case "G":
                    OrderByQuery = "Order By familysitememID Asc, Idx Desc";
                    break;
              
                default:
                    OrderByQuery = "Order by dt1Seq Desc";
                    break;
            }


            string Query =  @"	Select '" + chkNo + " '-row_Number() over(order by rowNum asc)	As trNo,* from (	"
                         + @"  Select row_Number() over("+ OrderByQuery + ")	As rowNum , * From (	"
                         + @"      Select  mb.mem_ID, mb.m_Name, mb.reg_Date, mb.m_Hp, bm.subProfID, bm.subProfName	"
                         + @"           , bm.familysitememID, od1.itemName, od1.payAmount, od1.authDate,od1.Status As orderStatus   "
                         + @"			, bm.Idx , A.lp_idx , Od1.dt1Seq , Lp.isMonthlyProduct "
                         + @"           , (Select top 1 fp_expire_Date From soobakc_test.dbo.tbl_Freepass_His Where fp_mem_ID = OD1.userID And O_ID = OD1.orderID) As fpExpireDate "
                         + @"           , (Select top 1 fp_isDel From soobakc_test.dbo.tbl_Freepass_His Where fp_mem_ID = OD1.userID And O_ID = OD1.orderID) As fpisDel , OD1.orderID"
                         + @"		From dbB2B.dbo.tblSoobakcB2BSalesItemList  as A		"
                         + @"			Inner Join dbSoobakc.dbo.tblOrderdetailLevel1 As Od1  on A.lp_Idx = Od1.lp_Idx	"
                         + @"			Inner Join soobakc_test.dbo.tbl_Member_basic As MB on OD1.userID = mb.mem_ID		"
                         + @"           Inner Join soobakc_test.dbo.tbl_lecture_Pack As Lp on OD1.lp_idx = LP.lp_Idx "
                         + @"			left outer join (	"
                         + @"  			Select * from dbb2b.dbo.tblSoobakcB2BmemInfo Where memStatus='1019' And companyID = @companyID  "
                         + @"          ) As Bm on mb.mem_ID = bm.mem_ID	"
                         + @"		Where A.isDel='N' and A.companyID = @companyID	"
                          + @"	)	As total	"
                         + @"	Where 1 = 1	" + subQuery
                         + @"	) as totalList where 1 = 1	"
                         + @"		And rowNum between ((@gotoPage - 1)* @pageSize + 1) and  (@gotoPage * @pageSize)	"
                         + @"    Order By rowNum Asc";

           
            var Parameters = new DynamicParameters(new { gotoPage = gotoPage, companyID = companyID, pageSize = pageSize, profInfo = profInfo, profInfoText = '%' + profInfoText + '%', productInfo = productInfo, memInfoCate = memInfoCate, memInfoText = '%' + memInfoText + '%', orderStatus = orderStatus, dateCategory = dateCategory, startDate = startDate, endDate = endDate });
            return con.Query<Sales>(Query, Parameters, commandType: CommandType.Text).ToList();
            
            
        }

        public List<SalesProductItem> GetProductList(int CompanyID)
        {
            
            string Query = @" Select lp.lp_idx, lp.lp_title, Si.salesYn "
                        + @"  From soobakc_test.dbo.tbl_lecture_Pack As lp"
                        + @"    Inner Join dbb2b.dbo.tblSoobakcb2bSalesItemList As si   "
                        + @"    on lp.lp_idx = si.lp_idx"
                        + @"  Where 1 = 1   "
                        + @"   And si.isDel = 'N' and si.useYn ='Y'"
                        + @"   And si.companyID = @companyID"
                        + @"  Order By lp.lp_idx Asc";

            var Parameters = new DynamicParameters(new {  companyID = CompanyID });
            return con.Query<SalesProductItem>(Query, Parameters, commandType: CommandType.Text).ToList();
        }
    }
}
