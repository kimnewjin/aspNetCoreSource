using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace wiseCamp.Models
{
   public class Member
    {
        public int ID { get; set; }
        public int rowNum { get; set; }
        public string mem_ID { get; set; }
        public string m_Name { get; set; }
        public string m_HP { get; set; }
        public string mainProfID { get; set; }
        public string mainProfName { get; set; }
        public string subProfID { get; set; }
        public string subProfName { get; set; }
        public string familysiteMemID { get; set; }
        public string familysiteMemName { get; set; }
        public string SiteGbName { get; set; }
        public string reg_date { get; set; }
        public int orderCount { get; set; }
        public int CancelCount { get; set; }
        public int Idx { get; set; }
        public string buyYnFlag { get; set; }
        public string EnterSiteGb { get; set; }      
        public int gotoPage { get; set; }
        public int pageSize { get; set; }
    }

    public class EidtSubPorfID
    {
        public int MIdx { get; set; }
        public string Mem_ID { get; set; }
        public string M_Name { get; set; }
        public string SubProfID { get; set; }
        public string SubProfName { get; set; }
    }


    public class MemberExcel
    {
      
        public string mem_ID { get; set; }
        public string m_Name { get; set; }
        public string m_HP { get; set; }
        public string mainProfID { get; set; }
        public string mainProfName { get; set; }
        public string subProfID { get; set; }
        public string subProfName { get; set; }
        public string familysiteMemID { get; set; }
        public string familysiteMemName { get; set; }
        public string SiteGbName { get; set; }
        public string reg_date { get; set; }
        public int orderCount { get; set; }
        public int CancelCount { get; set; }
        public int Idx { get; set; }
        public string buyYnFlag { get; set; }
        public string EnterSiteGb { get; set; }
     
    }


}
