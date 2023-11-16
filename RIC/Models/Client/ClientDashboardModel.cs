using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Client
{
    public class ClientDashboardModel
    {
        public int TotalSubmission { get; set; }
        public int TotalInterviews { get; set; }
        public int TotalHires { get; set; }
        public IEnumerable<ClintModelTable> ClientTable { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string EmpCd { get; set; }
    }

    public class ClintModelTable
    {
        [Display(Name = "Company")]
        public string Company { get; set; }

        [Display(Name = "Submissions")]
        public int Submissions { get; set; }

        [Display(Name = "Interviews")]
        public int Interviews { get; set; }

        [Display(Name = "Hires")]
        public int Hires { get; set; }        
    }
}