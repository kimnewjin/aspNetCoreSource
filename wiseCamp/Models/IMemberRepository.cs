using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wiseCamp.Models
{
    public interface IMemberRepository
    {
        List<Member> GetAll(int gotoPage, int pageSize, int companyID, string profInfo, string profInfoText, string memInfoCate, string memInfoText, string memStatus, string enterRoot, string memberStartDate, string memberEndDate, string OrderByFlag, int chkNo);
        int GetCountAll(int companyID, string profInfo, string profInfoText, string memInfoCate, string memInfoText, string memStatus, string enterRoot, string memberStartDate, string memberEndDate);
        EidtSubPorfID GetsubProfIDInfo(int memIdx);
        int goUpdateSubProfInfo(int memIdx, string mem_ID, string subProfID, string subProfName);
        List<MemberExcel> GetExcel(int companyID, string profInfo, string profInfoText, string memInfoCate, string memInfoText, string memStatus, string enterRoot, string memberStartDate, string memberEndDate, string OrderByFlag );





    }
}
