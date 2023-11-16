using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_MonthlyTarget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MT_Id { get; set; }
        public string Emp_Cd { get; set; }
        public string Mgr_Cd { get; set; }
        public string Emp_Name { get; set; }

        [DataType("decimal(16 ,3)")]
        public decimal Margin { get; set; }

        public string Account_Name { get; set; }

        public decimal Starts { get; set; }

        public string Month { get; set; }

        public int Year { get; set; }


        public int SubmissionTarget { get; set; }

        public int InterviewTarget { get; set; }

        public int HireTarget { get; set; }

        public string Status { get; set; }
       public string Role { get; set; }

        public string Team_Comments { get; set; }

        public string Supervisor_Comments { get; set; }

        public string Created_By { get; set; }

        public DateTime? Created_Date { get; set; }

        public string Updated_By { get; set; }

        public DateTime? Updated_Date { get; set; }


    }
}