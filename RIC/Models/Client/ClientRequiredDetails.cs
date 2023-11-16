using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Client
{
    public class ClientRequiredDetails
    {
        public string RJ_JobDiva_Ref { get; set; }

        public DateTime RJ_DateIssued { get; set; }
        public string RJ_Title { get; set; }
        public string RJ_Company { get; set; }
    }
}