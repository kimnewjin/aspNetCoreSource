using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.IO;







namespace wiseCamp.Dul
{

    

    public class BoardLibrary
    {
        private IConfiguration _config;
        private SqlConnection con;
        //   con = "Data Source = localhost; Initial Catalog = soobakc_Test; Persist Security Info=True;User ID = sa; Password=kimnew1024;";
       

        public BoardLibrary(IConfiguration config)
        {
            _config = config;
            con = new SqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
            
        }

        public static string FuncShowDate(object datetime)
        {
            string strDate = Convert.ToDateTime(datetime).ToString("yyyy-MM-dd");
            return strDate;
        }

        public static string FuncGetOrderMemberstatus(object fpisDel, object isMonthlyProduct, object orderID, object lpIdx, object fpExpireDate)
        {
            string dbConnection = "Data Source=localhost;Initial Catalog=soobakc_Test;Persist Security Info=True;User ID=sa;Password=kimnew1024;";

            return "김뉴";

        }



 
        public static  int FuncGetOrderCountType1(object AuthDate, object CategoryCode, string FeeStatus, object companyID, string FindLpIdx, string returnType)
        {
            string dbConnection = "Data Source=localhost;Initial Catalog=soobakc_Test;Persist Security Info=True;User ID=sa;Password=kimnew1024;";

          

            SqlConnection con = new SqlConnection(dbConnection);
                string subQuery = "";

                if (FeeStatus != "" && FeeStatus != null)
                {
                    switch (FeeStatus)
                    {
                        case "A":
                            subQuery = subQuery + "And Dateadd(month, 1, od.authDate) <= convert(date,getDate())";
                            break;
                        case "B":
                            subQuery = subQuery + "And Dateadd(month, 1, od.authDate) > convert(date,getDate())";
                            break;
                    }
                }

                if (FindLpIdx != "" && FindLpIdx != null)
                {
                    subQuery = subQuery + "And si.lp_Idx= '"+ FindLpIdx + "' ";
                }
                
                if (AuthDate != "" && AuthDate != null && CategoryCode != "" && CategoryCode != null)
                {
                    subQuery = subQuery + "And Convert(Date,authDate) = '"+ AuthDate +"' And si.lpCategory = '"+ CategoryCode + "'";
                }



                string Query = @"  Select	count(od.orderID) As odCount, isnull(sum(Od.payAmount),'0') As usePay	"
                                + @"  From dbB2B.dbo.tblSoobakcB2bSalesItemList As si		"
                                + @"      Inner Join dbSoobakc.dbo.tblOrderDetailLevel1 As od on Si.lp_Idx = Od.lp_Idx	"
                                + @"	Where si.isDel='N' And si.companyID= '"+ companyID + "'		"
                                + @"		And od.status='1' And Od.itemType='G'	"
                                + @"		And (	"
                                + @"			Select count(1) From  dbSoobakc.dbo.tblOrderDetailLevel1 As od1 	"
                                + @"			Where  od1.status='2' And od1.itemType='G' And od1.OrderId = OD.orderID "
                                + @"			And DateDiff(day, od.authdate, od1.AuthDate) < '1') =0	" + subQuery;
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                cmd.CommandText = Query;
                cmd.CommandType = CommandType.Text;

                SqlDataReader dr = cmd.ExecuteReader();
                int orderCount = 0;
                int usePay = 0;
                if (dr.Read())
                {
                //result = true;
                    orderCount  =  dr.GetInt32(0);//               .GetString(0);
                    usePay      =   dr.GetInt32(1);// dr.GetString(1);
                   
                    //  return "[" + orderCount + "]건<br/>" + usePay + "원";
                }

            con.Close();
                    if (returnType == "A")
                    {
                        return orderCount;
                    }
                    else
                    {
                        return usePay;
                    }

        }



        public static int FuncGetOrderCountType2(object AuthDate, object CategoryCode1, object CategoryCode2, string FeeStatus, object companyID, string FindLpIdx,  string returnType)
        {
            string dbConnection = "Data Source=localhost;Initial Catalog=soobakc_Test;Persist Security Info=True;User ID=sa;Password=kimnew1024;";



            SqlConnection con = new SqlConnection(dbConnection);
            string subQuery = "";

            if (FeeStatus != "" && FeeStatus != null)
            {
                switch (FeeStatus)
                {
                    case "A":
                        subQuery = subQuery + "And Dateadd(month, 1, od.authDate) <= convert(date,getDate())";
                        break;
                    case "B":
                        subQuery = subQuery + "And Dateadd(month, 1, od.authDate) > convert(date,getDate())";
                        break;
                }
            }

            if (FindLpIdx != "" && FindLpIdx != null)
            {
                subQuery = subQuery + "And si.lp_Idx= '" + FindLpIdx + "' ";
            }

            if (AuthDate != "" && AuthDate != null && CategoryCode1 != "" && CategoryCode1 != null && CategoryCode2 != "" && CategoryCode2 != null)
            {
                subQuery = subQuery + "And Convert(Date,authDate) = '" + AuthDate + "' And si.lpCategory In('" + CategoryCode1 + "','"+ CategoryCode2 + "')";
            }



            string Query = @"  Select	count(od.orderID) As odCount, isnull(sum(Od.payAmount),'0') As usePay	"
                            + @"  From dbB2B.dbo.tblSoobakcB2bSalesItemList As si		"
                            + @"      Inner Join dbSoobakc.dbo.tblOrderDetailLevel1 As od on Si.lp_Idx = Od.lp_Idx	"
                            + @"	Where si.isDel='N' And si.companyID= '" + companyID + "'		"
                            + @"		And od.status='1' And Od.itemType='G'	"
                            + @"		And (	"
                            + @"			Select count(1) From  dbSoobakc.dbo.tblOrderDetailLevel1 As od1 	"
                            + @"			Where  od1.status='2' And od1.itemType='G' And od1.OrderId = OD.orderID "
                            + @"			And DateDiff(day, od.authdate, od1.AuthDate) < '1') =0	" + subQuery;
           
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;
            cmd.CommandText = Query;
            cmd.CommandType = CommandType.Text;
            con.Open();
            SqlDataReader dr = cmd.ExecuteReader();
           
            int orderCount = 0;
            int usePay = 0;
            if (dr.Read())
            {
                //result = true;
                orderCount = dr.GetInt32(0);//               .GetString(0);
                usePay = dr.GetInt32(1);// dr.GetString(1);
               
            }
            con.Close();
            if (returnType == "A")
            {
                return orderCount;
            }
            else
            {
                return usePay;
            }

        }


    }
}
