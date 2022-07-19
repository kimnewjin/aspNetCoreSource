using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Extensions;



namespace wiseCamp.Models
{
    public class HomeRepository : IHomeRepository
    {
        private IConfiguration _config;
        private SqlConnection con;

        public HomeRepository(IConfiguration config)
        {
            _config = config;
            con = new SqlConnection(_config.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value);
        }

       
        //public bool IsCorrectUser(int CompanyID, string UserID, string PassWD, int ManagerCate)
        public string IsCorrectUser(string CompanyID, string UserID, string PassWD, string ManagerCate)
        {
            //bool result = false;
            string result = "";
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = con;

            cmd.CommandText = " Select BM.managerCate, BM.mem_ID, BM.m_Name"
                            + " From dbB2B.dbo.tblSoobakcB2BManager As BM    "
                            + "  Inner Join dbB2B.dbo.tblSoobakcB2BCompany As BC"
                            + "  on BC.companyID = BM.companyID "
                            + " Where BC.companyStatus ='Y' And BM.mDel ='N'    "
                            + " And BM.companyID = @companyID"
                            + " And BM.mem_ID = @userID "
                            + " And BM.m_PWD = @passWD  "
                            + " And BM.managerCate = @managerCate ";

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@companyID", CompanyID);
            cmd.Parameters.AddWithValue("@userID", UserID);
            cmd.Parameters.AddWithValue("@passWD", PassWD);
            cmd.Parameters.AddWithValue("@managerCate", ManagerCate);

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                //result = true;
                result = dr.GetString(2);
            }
            else
            {
                result = null;

               // result = "<script language='javascript'>alert(1);</script>";
            }

            con.Close();
            return result;
        }
    }
}
