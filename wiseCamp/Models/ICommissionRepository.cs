using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wiseCamp.Models
{
    public interface ICommissionRepository
    {
        List<Commission> GetCommisionList(int companyID, string startDate, string endDate, string productInfo, string feeStatus);
        List<SalesProductItem> GetProductList(int CompanyID);
        List<CommissionExcel> GetCommisionListExcel(int companyID, string startDate, string endDate, string productInfo, string feeStatus);
    }
}
