using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class RIC_Job_Report
    {
       [Key]
       public int RJ_ID { get; set; }

      // [Index("IX_RIC_Job_Report", 1, IsUnique = true)]
       [Display(Name="Employee ID ")]
       [Required]
       [StringLength(50)]       
       public string RJ_EmpCd { get; set; }

       [Display(Name = "Date Issued")]
       [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
       public DateTime RJ_DateIssued { get; set; }

       [Display(Name = "Title")]
       [StringLength(100)]
       public string RJ_Title { get; set; }

        [Display(Name = "Company")]
       [StringLength(100)]
       public string RJ_Company { get; set; }

        [Display(Name = "Department")]
       [StringLength(100)]
       public string RJ_Department { get; set; }

       //[Index("IX_RIC_Job_Report", 2, IsUnique = true)]
       [Display(Name = "Jobdiva Ref")]
       [StringLength(100)]
       public string RJ_JobDiva_Ref { get; set; }

       //[Index("IX_RIC_Job_Report", 3, IsUnique = true)]
       [Display(Name = "Optional Reference")]
       [StringLength(100)]
       public string RJ_Optional_Ref { get; set; }

       [Display(Name = "Priority")]
       [StringLength(100)]
       public string RJ_Priority { get; set; }

       [Display(Name = "Status")]
       [StringLength(100)]
       public string RJ_Job_Status { get; set; }

       [Display(Name = "Position Type")]
       [StringLength(100)]
       public string RJ_Position_Type { get; set; }

       [Display(Name = "Openings")]
       public int ? RJ_Openings { get; set; }

       [Display(Name = "Fills")]
       public int ? RJ_Fills { get; set; }

       [Display(Name = "Max Submittals")]
       public int ? RJ_Max_Submittals { get; set; }

       [Display(Name = "Manager")]
       [StringLength(100)]
       public string RJ_Manager { get; set; }

       [Display(Name = "Min Bill Rate")]
       public float ? RJ_Min_Bill_Rate { get; set; }

       [Display(Name = "Max Bill Rate")]
       public float ? RJ_Max_Bill_Rate { get; set; }

       [Display(Name = "Bill Rate Per")]
       [StringLength(20)]
       public string RJ_Bill_Rate_Per { get; set; }

        [Display(Name = "Min Pay Rate")]
       public float ? RJ_Min_Pay_Rate { get; set; }

       [Display(Name = "Max Pay Rate")]
       public float ? RJ_Max_Pay_Rate { get; set; }

       [Display(Name = "Pay Rate Per")]
       [StringLength(20)]
       public string RJ_Pay_Rate_Per { get; set; }

       [Display(Name = "Division")]
       [StringLength(50)]
       public string RJ_Division { get; set; }

       [Display(Name = "Address1")]
       [StringLength(500)]
       public string RJ_Address1 { get; set; }

       [Display(Name = "Address2")]
       [StringLength(500)]
       public string RJ_Address2 { get; set; }

       [Display(Name = "City")]
       [StringLength(100)]
       public string RJ_City { get; set; }

       [Display(Name = "State")]
       [StringLength(100)]
       public string RJ_State { get; set; }

       [Display(Name = "Zipcode")]
       [StringLength(100)]
       public string RJ_Zipcode { get; set; }

       [Display(Name = "Start Date")]
       [DataType(DataType.Date)]
       public DateTime ? RJ_Start_Date { get; set; }

       [Display(Name = "End Date")]
       [DataType(DataType.Date)]
       public DateTime ? RJ_End_Date { get; set; }

       [Display(Name = "Remarks")]
       [StringLength(500)]
       public string RJ_Remarks { get; set; }

       [Display(Name = "Skills")]
       public string RJ_Skills { get; set; }

       [Display(Name = "Prim Sales")]
       [StringLength(500)]
       public string RJ_Prim_Sales { get; set; }

       [Display(Name = "Prim Recruiter")]
       [StringLength(500)]
       public string RJ_Prim_Recruiter { get; set; }

       [Display(Name = "Users")]
       public string RJ_Users { get; set; }

       [Display(Name = "Candidate")]
       [StringLength(500)]
       public string RJ_Candidate { get; set; }

       [Display(Name = "Email")]
       [StringLength(500)]
       public string RJ_Email { get; set; }

       [Display(Name = "Alternate Email")]
       [StringLength(500)]
       public string RJ_Alt_Email { get; set; }

       [Display(Name = "Bill Rate")]
       public float ? RJ_Bill_Rate { get; set; }

        [Display(Name = "Pay Rate")]
       public float ? RJ_Pay_Rate { get; set; }

       [Display(Name = "C2C")]
       [StringLength(20)]
       public string RJ_C2C { get; set; }

       [Display(Name = "Submit Date")]
       [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
       public DateTime RJ_Submit_Date { get; set; }

       [Display(Name = "Interview Date")]
       [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
       public DateTime ? RJ_Interview_Date { get; set; }

       [Display(Name = "Hire Date")]
       [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy HH:mm}", ApplyFormatInEditMode = true)]
       public DateTime ? RJ_Hire_Date { get; set; }

       [Display(Name = "Elapsed Time")]
       //[DisplayFormat(DataFormatString = "{dd HH}", ApplyFormatInEditMode = true)]
       [StringLength(100)]
       public string RJ_Submit_Elapsed { get; set; }

       [Display(Name = "Elapsed Time")]
       //[DisplayFormat(DataFormatString = "{dd HH}", ApplyFormatInEditMode = true)]
       [StringLength(100)]
       public string  RJ_Interview_Elapsed { get; set; }

       [Display(Name = "Elapsed Time")]
       //[DisplayFormat(DataFormatString = "{dd HH}", ApplyFormatInEditMode = true)]
       [StringLength(100)]
       public string RJ_Hire_Elapsed { get; set; }



       [Display(Name = "Rejection Date")]
       public DateTime ? RJ_Rejection_Date { get; set; }

       [Display(Name = "Submitted By")]
       [StringLength(100)]
       public string RJ_Submitted_By { get; set; }

       [Display(Name = "Comments")]
       [StringLength(500)]
       public string RJ_Comments { get; set; }

       [Display(Name = "UploadedDate")]
       public DateTime RJ_UploadedDate { get; set; }
    }
}
