using DBLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.JobIssued
{
    public class JobIssuedVM
    {
        public IEnumerable<SelectListItem> ClientList { get; set; }
        public string Client { get; set; }

        public IEnumerable<SelectListItem> YearList { get; set; }
        public int Year { get; set; }

        public IEnumerable<SelectListItem> MonthList { get; set; }
        public int Month { get; set; }

        public DataTable JDIssued { get; set; }
    }

}