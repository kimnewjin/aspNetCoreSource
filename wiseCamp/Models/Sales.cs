using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wiseCamp.Models
{
    public class Sales
    {
        public string mem_ID { get; set; }
        public string m_Name { get; set; }
        public string reg_Date { get; set; }
        public string m_HP { get; set; }
        public string subProfID { get; set; }
        public string subProfName { get; set; }
        public string familysitememID { get; set; }
        public string itemName { get; set; }
        public string payAmount { get; set; }
        public string authDate { get; set; }
        public string orderStatus { get; set; }
        public int Idx { get; set; }
        public int rowNum { get; set; }
        public int lp_idx { get; set; }
        public string fpExpireDate { get; set; }
        public string fpisDel { get; set; }
        public int isMonthlyProduct { get; set; }
        public string orderID { get; set; }
        public int trNo { get; set; }

    }

    public class SalesProductItem
    {
        public string lp_idx { get; set; }
        public string lp_title { get; set; }
        public string salesYn { get; set; }
    }


    public class SalesExcel
    {
        public string mem_ID { get; set; }
        public string m_Name { get; set; }
        public string reg_Date { get; set; }
        public string m_HP { get; set; }
        public string subProfID { get; set; }
        public string subProfName { get; set; }
        public string familysitememID { get; set; }
        public string itemName { get; set; }
        public string payAmount { get; set; }
        public string authDate { get; set; }
        public string orderStatus { get; set; }
        public int Idx { get; set; }
        public int rowNum { get; set; }
        public int trNo { get; set; }

    }
}
