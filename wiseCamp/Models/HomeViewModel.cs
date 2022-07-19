using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace wiseCamp.Models
{
    public class HomeViewModel
    {
        public string CompanyID { get; set; }
        public string UserID { get; set; }
        public string PassWD { get; set; }
        public string ManagerCate { get; set; }
        
    }
}
