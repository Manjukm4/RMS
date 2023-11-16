using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RMS_SubmissionAnalysis
    {
        public int ID { get; set; }
        public string Emp_Cd { get; set; }
        public string Emp_Name { get; set; }
        public int? Sub_2017_Q4 { get; set; }
        public int? Sub_2018_Q1 { get; set; }
        public int? Sub_2018_Q2 { get; set; }
        public int? Sub_2018_Q3 { get; set; }
        public int? Sub_Total { get; set; }
        public int? Int_2017_Q4 { get; set; }
        public int? Int_2018_Q1 { get; set; }
        public int? Int_2018_Q2 { get; set; }
        public int? Int_2018_Q3 { get; set; }
        public int? Int_Total { get; set; }
        public int? Hires_2017_Q4 { get; set; }
        public int? Hires_2018_Q1 { get; set; }
        public int? Hires_2018_Q2 { get; set; }
        public int? Hires_2018_Q3 { get; set; }
        public int? Hire_Total { get; set; }

    }
}
