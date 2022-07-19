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
    public class CommissionRepository : ICommissionRepository
    {
        private IConfiguration _config;
        private SqlConnection con;
        private ILogger<CommissionRepository> _logger;

        public CommissionRepository(IConfiguration config, ILogger<CommissionRepository> logger)
        {
            _config = config;
            con = new SqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            _logger = logger;
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

            var Parameters = new DynamicParameters(new { companyID = CompanyID });
            return con.Query<SalesProductItem>(Query, Parameters, commandType: CommandType.Text).ToList();
        }


        public List<Commission> GetCommisionList(int companyID, string startDate, string endDate, string productInfo, string feeStatus)
        {
            string subQuery = "";
       
            if(startDate !="" && endDate != "" && startDate != null && endDate != null)
            {
                subQuery = subQuery + " And convert(date,authDate) between '" + startDate + "' And '" + endDate + "' ";
            }

            if(productInfo != "" && productInfo != null)
            {
                subQuery = subQuery + " And si.lp_idx = '" + productInfo + "' ";
            }

            if (feeStatus != "" && feeStatus != null)
            {
               switch (feeStatus)
                {
                    case "A":
                        subQuery = subQuery + " And DateAdd(month,1,od.authDate) <= convert(date,getDate()) ";
                        break;
                    case "B":
                        subQuery = subQuery + " And DateAdd(month,1,od.authDate) > convert(date,getDate()) ";
                        break;
                }
            }

            String Query = @"Select * ,Case when FeeDate < convert(date,getdate()) then '확정' else '미확정' end feestatus From ("
                         + @" Select *, Convert(Date,DateAdd(Month,1,orderDate)) As FeeDate From ( "
                         + @"   Select convert(date, authDate) As orderDate,  Count(distinct od.userID) As memCount,  "
                         + @"       count(od.orderID) As orderCount, sum(Od.payAmount) As payAmout	"
                         + @"	From dbB2B.dbo.tblSoobakcB2bSalesItemList As si	"
                         + @"	    Inner Join dbSoobakc.dbo.tblOrderDetailLevel1 As od on Si.lp_Idx = Od.lp_Idx	"
                         + @"	Where si.isDel='N' And si.companyID= @companyID	"
                         + @"		And od.status='1' And Od.itemType='G'	" + subQuery
                         + @"   And (	"
                         + @"   	Select count(1) From dbB2B.dbo.tblSoobakcB2bSalesItemList As si	"
                         + @"           Inner Join dbSoobakc.dbo.tblOrderDetailLevel1 As od1 on Si.lp_Idx = od1.lp_Idx	"
                         + @"   Where si.isDel='N' And si.companyID=@companyID	"
                         + @"				And od1.status='2' And od1.itemType='G' And od1.OrderId = OD.orderID	"
                         + @"       And DateDiff(day, od.authdate, od1.AuthDate) < '1') =0	"
                         + @"	Group by convert(date,authDate) ) As total ) as totalList	";
            var Parameters = new DynamicParameters(new { companyID = companyID });
            return con.Query<Commission>(Query, Parameters, commandType: CommandType.Text).ToList();
        }

        public List<CommissionExcel> GetCommisionListExcel(int companyID, string startDate, string endDate, string productInfo, string feeStatus)
        {
            string subQuery = "";

            if (startDate != "" && endDate != "" && startDate != null && endDate != null)
            {
                subQuery = subQuery + " And convert(date,authDate) between '" + startDate + "' And '" + endDate + "' ";
            }

            if (productInfo != "" && productInfo != null)
            {
                subQuery = subQuery + " And si.lp_idx = '" + productInfo + "' ";
            }

            if (feeStatus != "" && feeStatus != null)
            {
                switch (feeStatus)
                {
                    case "A":
                        subQuery = subQuery + " And DateAdd(month,1,od.authDate) <= convert(date,getDate()) ";
                        break;
                    case "B":
                        subQuery = subQuery + " And DateAdd(month,1,od.authDate) > convert(date,getDate()) ";
                        break;
                }
            }

            String Query = @"Select * ,Case when FeeDate < convert(date,getdate()) then '확정' else '미확정' end feestatus From ("
                         + @" Select *, Convert(Date,DateAdd(Month,1,orderDate)) As FeeDate From ( "
                         + @"   Select convert(date, authDate) As orderDate,  Count(distinct od.userID) As memCount,  "
                         + @"       count(od.orderID) As orderCount, sum(Od.payAmount) As payAmout	"
                         + @"	From dbB2B.dbo.tblSoobakcB2bSalesItemList As si	"
                         + @"	    Inner Join dbSoobakc.dbo.tblOrderDetailLevel1 As od on Si.lp_Idx = Od.lp_Idx	"
                         + @"	Where si.isDel='N' And si.companyID= @companyID	"
                         + @"		And od.status='1' And Od.itemType='G'	" + subQuery
                         + @"   And (	"
                         + @"   	Select count(1) From dbB2B.dbo.tblSoobakcB2bSalesItemList As si	"
                         + @"           Inner Join dbSoobakc.dbo.tblOrderDetailLevel1 As od1 on Si.lp_Idx = od1.lp_Idx	"
                         + @"   Where si.isDel='N' And si.companyID=@companyID	"
                         + @"				And od1.status='2' And od1.itemType='G' And od1.OrderId = OD.orderID	"
                         + @"       And DateDiff(day, od.authdate, od1.AuthDate) < '1') =0	"
                         + @"	Group by convert(date,authDate) ) As total ) as totalList	";
            var Parameters = new DynamicParameters(new { companyID = companyID });
            return con.Query<CommissionExcel>(Query, Parameters, commandType: CommandType.Text).ToList();
        }


    }
}
