using System;

using Microsoft.AspNetCore.Razor.TagHelpers;

namespace wiseCamp.TagHelpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
 
    public class DnnPagingHelperTagHelper : TagHelper
    {

        public bool SearchMode { get; set; } = false;

        public string profInfo { get; set; }
        public string profInfoText { get; set; }
        public string memInfoCate { get; set; }
        public string memInfoText { get; set; }
        public string memStatus { get; set; }
        public string enterRoot { get; set; }
        public string memberStartDate { get; set; }
        public string memberEndDate { get; set; }

       


        public int PageIndex { get; set; } = 0;
        public int PageCount { get; set; }
        public int PageSize { get; set; }

        public string Url { get; set; }

        private int _RecordCount;

        public int RecordCount
        {
            get { return _RecordCount; }
            set
            {
                _RecordCount = value;
                PageCount = ((_RecordCount - 1) / PageSize) + 1;
            }
        }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "ul";
            output.Attributes.Add("class", "pagination");

            if (PageIndex == 0)
            {
                PageIndex = 1;
            }

            int i = 0;

            string strPage = "";

            if (PageIndex > 10)
            {
                if (!SearchMode)
                {
                    strPage += "<li><a hre=\"" + Url + "?gotoPage=" + Convert.ToString(((PageIndex - 1) / (int)10) * 10) + "\">◀</a></li>";
                }
                else
                {
                    /*검색 파라미터 추가*/
                    strPage += "<li><a hre=\"" + Url + "?gotoPage=" + Convert.ToString(((PageIndex - 1) / (int)10) * 10) + "\">◀</a></li>";
                }
            }
            else
            {
                strPage += "<li class='disabled'><a>◁</a></li>";
            }

            for( i = (((PageIndex - 1) /(int)10)*10 +1); i<=((((PageIndex - 1) / (int)10) + 1) * 10); i++)
            {
                if(i> PageCount)
                {
                    break;
                }
                if(i == PageIndex)
                {
                    strPage += "<li class='active'><a href='#'>" + i.ToString() + "</a></li>";
                }
                else
                {
                    if (!SearchMode)
                    {
                        strPage += "<li><a href=\""+Url+"?gotoPage="+i.ToString()+"\">"+i.ToString()+"</a></li>";
                    }
                    else
                    {
                        /*검 색 파라미터 입력*/
                        strPage += "<li><a href=\"" + Url + "?gotoPage=" + i.ToString() + "\">" + i.ToString() + "</a></li>";
                    }
                }
            }

            if (i < PageCount)
            {
                if (!SearchMode)
                {
                    strPage += "<li><a href=\"" + Url + "?gotoPage=" + Convert.ToString(((PageIndex - 1) / (int)10) * 10 + 11) + "\">▶</a></li>";
                }
                else
                {
                    /*검색어 입력*/
                    strPage += "<li><a href=\"" + Url + "?gotoPage=" + Convert.ToString(((PageIndex - 1) / (int)10) * 10 + 11) + "\">▶</a></li>";
                }
            }
            else
            {
                strPage += "<li class='disabled'><a>▷</a></li>";
            }

            output.Content.AppendHtml(strPage);

        }
    }
}
