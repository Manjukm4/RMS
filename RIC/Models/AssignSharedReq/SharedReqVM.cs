using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.AssignSharedReq
{
    public class AssignSharedReqVM
    {
        public List<AssignSharedReq> ListSharedReq { get; set; }
        public AssignSharedReqSearch SharedReqSearch { get; set; }
        public string Message { get; set; }

        public IEnumerable<SelectListItem> TeamLeads { get; set; }
        

    }
}