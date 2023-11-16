using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages.Html;
using RIC.Models.Dashboard;

namespace RIC.CustomHelper
{
    public static class CustomHelper
    {
        public static HtmlString TimeSpanString( TimeSpan ? val)
        {
            if (val.HasValue)
            {
                //var formattedString = string.Format("{0}:{1}", ((int)val.Value.TotalHours), val.Value.Minutes, val.Value.Seconds);
                var formattedString = ((int)val.Value.TotalHours).ToString("00") + ":" + val.Value.Minutes.ToString("00");
                return new HtmlString("<span>" + formattedString + "</span>");
                //double days = val.Value.Days;
                //double hours = val.Value.Hours + (days * 24);
                //double minutes = val.Value.Minutes;
                //double seconds = val.Value.Seconds;
                //formattedString = String.Format("{0:00}:{1:00}, hours, minutes");
            }
            else 
            {
                return null;            
            }            
        }
        //get employee count.
        public static int GetCountFromTreePartial(IEnumerable<TreeViewPartial> tree,string empCd,int count = 0)
        {            
            var items=tree.Where(w=>w.MgrCD==empCd);
            foreach (var item in items)
            {
                count++;
                count += GetCountFromTreePartial(tree, item.EmpCD);
            }   
            return count;
        }

    }
}