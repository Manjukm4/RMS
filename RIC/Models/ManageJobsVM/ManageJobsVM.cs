using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.ManageJobsVM
{
    public class ManageJobsVM
    {
        public List<Jobs> Listjobs { get; set; }
        public IEnumerable<SelectListItem> Recruiters { get; set; }
        public int JobsCheckedOut { get; set; }
        public string Message { get; set; }
    }
}