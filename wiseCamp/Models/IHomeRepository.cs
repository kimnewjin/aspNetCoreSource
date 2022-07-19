using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace wiseCamp.Models
{
    public interface IHomeRepository
    {
        //bool IsCorrectUser(int CompanyID, string UserID, string PassWD, int ManagerCate);
        string IsCorrectUser(string CompanyID, string UserID, string PassWD, string ManagerCate);
    }
}
