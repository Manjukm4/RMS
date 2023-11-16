using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Account
{
    public class EditReportingHistoryVM
    {
        public int Id { get; set; }

        [Display(Name = "Jobdiva User Name")]
        public string JobdivaUserName { get; set; }

        public string EmployeeCd { get; set; }

        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

        public string ManagerId { get; set; }

        [Display(Name = "Reporting To")]
        public List<SelectListItem> ManagerList { get; set; }

    }
}