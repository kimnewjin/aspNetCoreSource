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

namespace wiseCamp.Models
{
    public class MemberRepository : IMemberRepository
    {
        private IConfiguration _config;
        private SqlConnection con;
        private ILogger<MemberRepository> _logger;

        public MemberRepository(IConfiguration config, ILogger<MemberRepository> logger)
        {
            _config = config;
            con = new SqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            _logger = logger;
        }

        public int GetCountAll(int companyID, string profInfo, string profInfoText, string memInfoCate, string memInfoText, string memStatus, string enterRoot, string memberStartDate, string memberEndDate)
        {
          /* try
            {*/
                string subQuery = "";

             if (profInfo !="" && profInfoText != ""){
                switch (profInfo)
                {
                    case "total.mainprofID":
                        subQuery = subQuery + " And total.mainprofID like @profInfoText ";
                        break;
                    case "total.subprofID":
                        subQuery = subQuery + " And total.subprofID like @profInfoText ";
                        break;
                }
                
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

            if(memStatus != "")
            {
                switch (memStatus)
                {
                    case "B":/*가입이전*/
                        subQuery = subQuery + " And total.mem_ID is Null ";
                        break;
                    case "F":
                        subQuery = subQuery + " And total.mem_ID is Not Null ";
                        subQuery = subQuery + "	And (Select Count(1) From dbB2b.dbo.ViewLectureHistorySoobakc Where mem_Id = total.Mem_Id) = 0	";
                        subQuery = subQuery + "	And total.orderCount = 0 And total.CancelCount = 0";
                        break;
                    case "N":
                        subQuery = subQuery + "	And total.Mem_Id Is Not Null	";
                        subQuery = subQuery + "	And total.buyYnflag = 'N'	";
                        subQuery = subQuery + "	And total.orderCount = '0' ";                        
                        break;
                    case "BV":
                        subQuery = subQuery + "	And total.orderCount <> '0' 	";
                        subQuery = subQuery + "	And total.CancelCount = 0	";
                        break;
                    case "BC":
                        subQuery = subQuery + "	And total.CancelCount = 0	";
                        break;
                }
            }


            if (@memberStartDate != null && @memberEndDate != null)
            {                
                subQuery = subQuery + "	And datediff(day,total.Reg_Date, getDate()) Between datediff(day,@memberEndDate ,getdate()) And datediff(day,@memberStartDate ,getdate()) ";
            }

      

       




                string Query = @"	Select Count(1) from (	"
                         + @"	    Select * , row_Number() over(Order by Idx Desc)	As rowNum	 From (	"
                         + @"		    Select mb.mem_Id, mb.m_Name, mb.m_HP, Bm.mainProfID, Bm.mainProfNAme, BM.subProfID, BM.subProfName, BM.familysiteMemID 	"
                         + @"			,	bm.FamilysiteMemName, (Select tName From dbb2b.dbo.tblSoobakcb2bCode where tCode =entersiteGB) As SiteGbName	"
                         + @"			,	mb.reg_date		"
                         + @"			, (	Select Count(1) From  dbSoobakc.dbo.tblOrderDetailLevel1 As A	"
                         + @"					Inner Join dbB2B.dbo.tblSoobakcB2BSalesItemList  As B on A.LP_Idx = B.lp_idx	"
                         + @"				Where B.isDel='N' And B.companyID= @companyID	"
                         + @"					and status='1' And isRefund='0'  And userID = bm.mem_ID)	As orderCount	"
                         + @"			, (	Select Count(1) From  dbSoobakc.dbo.tblOrderDetailLevel1 As A	"
                         + @"					Inner Join dbB2B.dbo.tblSoobakcB2BSalesItemList  As B on A.LP_Idx = B.lp_idx	"
                         + @"				Where B.isDel='N' And B.companyID= @companyID	"
                         + @"					and status='2' And isRefund='1'  And userID = bm.mem_ID)	As CancelCount, bm.Idx	"
                         + @"					, isnull(bm.buyYnFlag,'') as buyYnFlag, bm.EnterSiteGb	"
                         + @"		From dbb2b.dbo.tblSoobakcb2bMemInfo As Bm	"
                         + @"			Left Outer Join soobakc_Test.dbo.tbl_Member_basic As MB on Bm.mem_Id = Mb.mem_ID	"
                         + @"		Where Bm.isDel='N' 	And bm.companyID	=  @companyID			"
                         + @"	)	As total	"
                         + @"	Where 1 = 1	" + subQuery
                         + @"	) as totalList where 1 = 1	";


                 return con.Query<int>(Query, new { companyID = companyID , profInfo= profInfo, profInfoText  = '%'+ profInfoText + '%',  memInfoText = '%' + memInfoText + '%' , memberEndDate= memberEndDate , memberStartDate = memberStartDate }, commandType: CommandType.Text).SingleOrDefault();

               /* }
                catch
                {
                    _logger.LogError("에러발생");
                    return -1;
                }*/
            }

        public int goUpdateSubProfInfo(int memIdx, string mem_ID, string subProfID, string subProfName)
        {
            try
            {
                string Query = @"UpDate dbB2B.dbo.tblSoobakcB2bMemInfo Set"
                        + @" subProfID = @subProfID  "
                        + @" , subProfName = @subProfName"
                        + @" Where Idx=@memIdx    "
                        + @" And mem_ID = @mem_ID";

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = Query;
                cmd.CommandType = CommandType.Text;

                cmd.Parameters.AddWithValue("@subProfID", subProfID);
                cmd.Parameters.AddWithValue("@subProfName", subProfName);
                cmd.Parameters.AddWithValue("@memIdx", memIdx);
                cmd.Parameters.AddWithValue("@mem_ID", mem_ID);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                return 1;
            }
            catch
            {
                _logger.LogError("에러발생");
                return -1;
            }

        }

        public EidtSubPorfID GetsubProfIDInfo(int memIdx)
        {
            EidtSubPorfID ESP = new EidtSubPorfID();

            string Query = @"Select A.Idx as mIdx, A.mem_ID, B.m_Name, A.subprofID, A.subProfName from dbB2B.dbo.tblSoobakcB2bMemInfo As A"
                        +@" Inner Join soobakc_test.dbo.tbl_Member_Basic As B on A.mem_ID = B.mem_ID "
                        + @" Where A.Idx = @memIdx ";
           /* var Parameters = new DynamicParameters(new { memIdx = memIdx });
            return con.Query<EidtSubPorfID>(Query, Parameters, commandType: CommandType.Text).ToList();*/

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = Query;
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.AddWithValue("@memIdx", memIdx);
            con.Open();
            IDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                ESP.MIdx = dr.GetInt32(0);
                ESP.Mem_ID = dr.GetString(1);
                ESP.M_Name = dr.GetString(2);
                ESP.SubProfID = dr.GetString(3);
                ESP.SubProfName = dr.GetString(4);
            }

            con.Close();
            return ESP;

        }

        public List<Member> GetAll(int gotoPage, int pageSize, int companyID, string profInfo, string profInfoText, string memInfoCate, string memInfoText, string memStatus, string enterRoot, string memberStartDate, string memberEndDate, string OrderByFlag, int chkNo)
        {
            _logger.LogInformation("데이터 출력");
            try
            {

                string subQuery = "";

                if (profInfo != "" && profInfoText != "")
                {
                    switch (profInfo)
                    {
                        case "total.mainprofID":
                            subQuery = subQuery + " And total.mainprofID like @profInfoText ";
                            break;
                        case "total.subprofID":
                            subQuery = subQuery + " And total.subprofID like @profInfoText ";
                            break;
                    }

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


                if (memStatus != "")
            {
                switch (memStatus)
                {
                    case "B":/*가입이전*/
                        subQuery = subQuery + " And total.mem_ID is Null ";
                        break;
                    case "F":
                        subQuery = subQuery + " And total.mem_ID is Not Null ";
                        subQuery = subQuery + "	And (Select Count(1) From dbB2b.dbo.ViewLectureHistorySoobakc Where mem_Id = total.Mem_Id) = 0	";
                        subQuery = subQuery + "	And total.orderCount = 0 And total.CancelCount = 0";
                        break;
                    case "N":
                        subQuery = subQuery + "	And total.Mem_Id Is Not Null	";
                        subQuery = subQuery + "	And total.buyYnflag = 'N'	";
                        subQuery = subQuery + "	And total.orderCount = '0' ";
                        break;
                    case "BV":
                        subQuery = subQuery + "	And total.orderCount <> '0' 	";
                        subQuery = subQuery + "	And total.CancelCount = 0	";
                        break;
                    case "BC":
                        subQuery = subQuery + "	And total.CancelCount = 0	";
                        break;
                }
            }


                if (@memberStartDate != null && @memberEndDate != null)
            {
                subQuery = subQuery + "	And datediff(day,total.Reg_Date, getDate()) Between datediff(day,@memberEndDate ,getdate()) And datediff(day,@memberStartDate ,getdate()) ";
            }

                string OrderByQuery = "";

                switch (OrderByFlag)
            {
                case "A":
                    OrderByQuery = "Order by Idx Desc";
                    break;
                case "B":
                    OrderByQuery = "Order by mem_ID Asc, Idx Desc";
                    break;
                case "C":
                    OrderByQuery = "Order by m_Name Asc, Idx Desc";
                    break;
                case "D":
                    OrderByQuery = "Order by m_Hp Asc, Idx Desc";
                    break;
                case "E":
                    OrderByQuery = "Order by mainProfID Asc , Idx Desc";
                    break;
                case "F":
                    OrderByQuery = "Order by mainProfName Asc, Idx Desc";
                    break;
                case "G":
                    OrderByQuery = "Order By subProfID Asc, Idx Desc";
                    break;
                case "H":
                    OrderByQuery = "Order By subProfName Asc, Idx Desc";
                    break;
                case "I":
                    OrderByQuery = "Order By FamilysitememID Asc, Idx Desc";
                    break;
                case "J":
                    OrderByQuery = "Order by FamilySitememName Asc, Idx Desc";
                    break;
                default:
                     OrderByQuery = "Order by Idx Desc";
                    break;
            }

                chkNo = chkNo + 1;


                string Query = @"	Select '" + chkNo + " '-row_Number() over(order by rowNum asc)	As ID,* from (	"
                         + @"	    Select row_Number() over(" + OrderByQuery + ")	As rowNum ,*  	 From (	"
                         + @"		    Select mb.mem_Id, mb.m_Name, mb.m_HP, Bm.mainProfID, Bm.mainProfName, BM.subProfID, BM.subProfName, BM.familysiteMemID 	"
                         + @"			,	bm.familysiteMemName, (Select tName From dbb2b.dbo.tblSoobakcb2bCode where tCode =entersiteGB) As SiteGbName	"
                         + @"			,	mb.reg_date		"
                         + @"			, (	Select Count(1) From  dbSoobakc.dbo.tblOrderDetailLevel1 As A	"
                         + @"					Inner Join dbB2B.dbo.tblSoobakcB2BSalesItemList  As B on A.LP_Idx = B.lp_idx	"
                         + @"				Where B.isDel='N' And B.companyID= @companyID	"
                         + @"					and status='1' And isRefund='0'  And userID = bm.mem_ID)	As orderCount	"
                         + @"			, (	Select Count(1) From  dbSoobakc.dbo.tblOrderDetailLevel1 As A	"
                         + @"					Inner Join dbB2B.dbo.tblSoobakcB2BSalesItemList  As B on A.LP_Idx = B.lp_idx	"
                         + @"				Where B.isDel='N' And B.companyID= @companyID	"
                         + @"					and status='2' And isRefund='1'  And userID = bm.mem_ID)	As CancelCount, bm.Idx	"
                         + @"					, isnull(bm.buyYnFlag,'') as buyYnFlag, bm.EnterSiteGb , @gotoPage As gotoPage, @pageSize As pageSize	"
                         + @"		From dbb2b.dbo.tblSoobakcb2bMemInfo As Bm	"
                         + @"			Left Outer Join soobakc_Test.dbo.tbl_Member_basic As MB on Bm.mem_Id = Mb.mem_ID	"
                         + @"		Where Bm.isDel='N' 	And bm.companyID	=  @companyID			"
                         + @"	)	As total	"
                         + @"	Where 1 = 1	" + subQuery
                         + @"	) as totalList where 1 = 1	"
                         + @"		And rowNum between ((@gotoPage - 1)* @pageSize + 1) and  (@gotoPage * @pageSize)	"
                         + @"    Order By rowNum Asc";

               
                
                var Parameters = new DynamicParameters(new { gotoPage = gotoPage, companyID = companyID, pageSize = pageSize, profInfo = profInfo, profInfoText = '%' + profInfoText + '%' ,  memInfoText = '%' + memInfoText + '%', memberEndDate = memberEndDate, memberStartDate = memberStartDate });
                return con.Query<Member>(Query, Parameters, commandType: CommandType.Text).ToList();
            }
            catch(System.Exception ex)
            {
                _logger.LogError("데이터 출력 에러:" + ex);
                return null;
            }
        }

        public List<MemberExcel> GetExcel(int companyID, string profInfo, string profInfoText, string memInfoCate, string memInfoText, string memStatus, string enterRoot, string memberStartDate, string memberEndDate, string OrderByFlag)
        {

            
            _logger.LogInformation("데이터 출력");
        

                string subQuery = "";

                if (profInfo != "" && profInfoText != "")
                {
                    switch (profInfo)
                    {
                        case "total.mainprofID":
                            subQuery = subQuery + " And total.mainprofID like @profInfoText ";
                            break;
                        case "total.subprofID":
                            subQuery = subQuery + " And total.subprofID like @profInfoText ";
                            break;
                    }

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


                if (memStatus != "")
                {
                    switch (memStatus)
                    {
                        case "B":/*가입이전*/
                            subQuery = subQuery + " And total.mem_ID is Null ";
                            break;
                        case "F":
                            subQuery = subQuery + " And total.mem_ID is Not Null ";
                            subQuery = subQuery + "	And (Select Count(1) From dbB2b.dbo.ViewLectureHistorySoobakc Where mem_Id = total.Mem_Id) = 0	";
                            subQuery = subQuery + "	And total.orderCount = 0 And total.CancelCount = 0";
                            break;
                        case "N":
                            subQuery = subQuery + "	And total.Mem_Id Is Not Null	";
                            subQuery = subQuery + "	And total.buyYnflag = 'N'	";
                            subQuery = subQuery + "	And total.orderCount = '0' ";
                            break;
                        case "BV":
                            subQuery = subQuery + "	And total.orderCount <> '0' 	";
                            subQuery = subQuery + "	And total.CancelCount = 0	";
                            break;
                        case "BC":
                            subQuery = subQuery + "	And total.CancelCount = 0	";
                            break;
                    }
                }


                if (@memberStartDate != null && @memberEndDate != null)
                {
                    subQuery = subQuery + "	And datediff(day,total.Reg_Date, getDate()) Between datediff(day,@memberEndDate ,getdate()) And datediff(day,@memberStartDate ,getdate()) ";
                }

                string OrderByQuery = "";

                switch (OrderByFlag)
                {
                    case "A":
                        OrderByQuery = "Order by Idx Desc";
                        break;
                    case "B":
                        OrderByQuery = "Order by mem_ID Asc, Idx Desc";
                        break;
                    case "C":
                        OrderByQuery = "Order by m_Name Asc, Idx Desc";
                        break;
                    case "D":
                        OrderByQuery = "Order by m_Hp Asc, Idx Desc";
                        break;
                    case "E":
                        OrderByQuery = "Order by mainProfID Asc , Idx Desc";
                        break;
                    case "F":
                        OrderByQuery = "Order by mainProfName Asc, Idx Desc";
                        break;
                    case "G":
                        OrderByQuery = "Order By subProfID Asc, Idx Desc";
                        break;
                    case "H":
                        OrderByQuery = "Order By subProfName Asc, Idx Desc";
                        break;
                    case "I":
                        OrderByQuery = "Order By FamilysitememID Asc, Idx Desc";
                        break;
                    case "J":
                        OrderByQuery = "Order by FamilySitememName Asc, Idx Desc";
                        break;
                    default:
                        OrderByQuery = "Order by Idx Desc";
                        break;
                }




            string Query = @"	    Select *  	 From (	"
                     + @"		    Select mb.mem_Id, mb.m_Name, mb.m_HP, Bm.mainProfID, Bm.mainProfName, BM.subProfID, BM.subProfName, BM.familysiteMemID 	"
                     + @"			,	bm.familysiteMemName, (Select tName From dbb2b.dbo.tblSoobakcb2bCode where tCode =entersiteGB) As SiteGbName	"
                     + @"			,	mb.reg_date		"
                     + @"			, (	Select Count(1) From  dbSoobakc.dbo.tblOrderDetailLevel1 As A	"
                     + @"					Inner Join dbB2B.dbo.tblSoobakcB2BSalesItemList  As B on A.LP_Idx = B.lp_idx	"
                     + @"				Where B.isDel='N' And B.companyID= @companyID	"
                     + @"					and status='1' And isRefund='0'  And userID = bm.mem_ID)	As orderCount	"
                     + @"			, (	Select Count(1) From  dbSoobakc.dbo.tblOrderDetailLevel1 As A	"
                     + @"					Inner Join dbB2B.dbo.tblSoobakcB2BSalesItemList  As B on A.LP_Idx = B.lp_idx	"
                     + @"				Where B.isDel='N' And B.companyID= @companyID	"
                     + @"					and status='2' And isRefund='1'  And userID = bm.mem_ID)	As CancelCount, bm.Idx	"
                     + @"					, isnull(bm.buyYnFlag,'') as buyYnFlag, bm.EnterSiteGb 	"
                     + @"		From dbb2b.dbo.tblSoobakcb2bMemInfo As Bm	"
                     + @"			Left Outer Join soobakc_Test.dbo.tbl_Member_basic As MB on Bm.mem_Id = Mb.mem_ID	"
                     + @"		Where Bm.isDel='N' 	And bm.companyID	=  @companyID			"
                     + @"	)	As total	"
                     + @"	Where 1 = 1	" + subQuery + OrderByQuery;
                        



                var Parameters = new DynamicParameters(new {  companyID = companyID,  profInfo = profInfo, profInfoText = '%' + profInfoText + '%', memInfoText = '%' + memInfoText + '%', memberEndDate = memberEndDate, memberStartDate = memberStartDate });
                return con.Query<MemberExcel>(Query, Parameters, commandType: CommandType.Text).ToList();
           
        }


    }
}
