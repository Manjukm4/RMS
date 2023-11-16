using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Client
{
    public class ClientMapping
    {
        public int RCM_ClientId { get; set; }
        
        public int EmpId { get; set; }


        public DateTime RCM_StartDate { get; set; }

        public DateTime? RCM_EndDate { get; set; }

        public string ClinetName { get; set; }

        public string AccountManagerName { get; set; }

        public string TeamLeadName { get; set; }

        public IEnumerable<SelectListItem> ClientList { get; set; }

        public IEnumerable<SelectListItem> ClientList2 { get; set; }

    }
}