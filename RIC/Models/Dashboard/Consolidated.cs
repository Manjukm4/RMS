using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models.Dashboard
{
	public class Consolidated
	{
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime IssuedDate { get; set; }
		public string JobDiva_Ref { get; set; }
        public string Title { get; set; }
		public string Company { get; set; }
		public int Count { get; set; }
        public string Client_Ref { get; set; }
    }

    public class DetailsByUser 
    {
        public string HeaderText { get; set; }
        public string EmpCd { get; set; }
        public string  EmployeeName { get; set; }
        public int Count { get; set; }
        public int Submissons { get; set; }
        public int Interviews { get; set; }
        public int Hires { get; set; }


    }
//    public class AccessLogDetails
//    {
     
//        public string EmpCd { get; set; }
//        public string EmployeeName { get; set; }
//        public string Category { get; set; }
//        public DateTime? RC_Date { get; set; }
//        public string Description { get; set; }
        
//    }
//    public class AccessLogSummary
//    {
//        public string EmpCd { get; set; }
//        public string EmployeeName { get; set; }
//        public string Category { get; set; }

//        public string Category_Count { get; set; }
//        public DateTime? RC_Date { get; set; }
//        public DateTime? FirstLogin_Time { get; set; }
            
//        public DateTime? LastLogin_Time { get; set; }
        
//        public int Duration { get; set; }


//}

}