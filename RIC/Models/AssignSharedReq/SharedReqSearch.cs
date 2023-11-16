using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.AssignSharedReq
{
    public class AssignSharedReqSearch
    {
        public string ClientID { get; set; }
        public string CompanyName { get; set; }

        public string JobDivaRef { get; set; }
        public string JobTitle { get; set; }
        public DateTime JobIssuedDate { get; set; }

    }
}