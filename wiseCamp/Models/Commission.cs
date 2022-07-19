using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wiseCamp.Models
{
    public class Commission
    {
        public string orderDate { get; set; }
        public string FeeDate { get; set; }
        public int memCount { get; set; }
        public int orderCount { get; set; }
        public int payAmout { get; set; }
        public string feeStatus { get; set; }
        public int FeeAmout { get; set; }
    }

    public class CommissionExcel
    {
        public string orderDate { get; set; }
        public string FeeDate { get; set; }
        public int memCount { get; set; }
        public int orderCount { get; set; }
        public int payAmout { get; set; }
        public string feeStatus { get; set; }
        public int FeeAmout { get; set; }
    }

}
