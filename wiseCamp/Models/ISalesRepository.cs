using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wiseCamp.Models
{
    public interface ISalesRepository
    {
        List<Sales> GetOrderList(int gotoPage, int pageSize, int companyID, string profInfo, string profInfoText, string productInfo, string memInfoCate, string memInfoText, string orderStatus, string dateCategory, string startDate, string endDate, int chkNo, string orderByflag);        
        List<SalesProductItem> GetProductList(int CompanyID);
       int GetOrderCount(int companyID, string profInfo, string profInfoText, string productInfo, string memInfoCate, string memInfoText, string orderStatus, string dateCategory, string startDate, string endDate);
        List<SalesExcel> GetSalesExcel(int CompanyID, string profInfo, string profInfoText, string productInfo, string memInfoCate, string memInfoText, string orderStatus, string dateCategory, string startDate, string endDate, string orderByflag);
                                                                                                                       
    }
}
