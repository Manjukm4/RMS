using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.SharedReq
{
    public class SharedReqVM
    {
        public List<SharedReq> ListSharedReq { get; set; }
        public SharedReqSearch SharedReqSearch { get; set; }
        public string Message { get; set; } 
    }
}