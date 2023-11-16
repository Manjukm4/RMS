using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DBLibrary
{
    //[MetadataType(typeof(RIC_EmployeeMetadata))]
    public partial class RIC_Employee
    {

        public RIC_Employee()
        {
            this.RIC_User_Role = new List<RIC_User_Role>();
            this.RIC_EmployeeNav = new List<RIC_Employee>();
        }

        [Column(TypeName = "numeric")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RE_EmpId { get; set; }

        [ForeignKey("RIC_SubmissionRule")]
        public int RE_Sub_Rule_ID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Employee ID")]
        public string RE_Emp_Cd { get; set; }

        [Column(TypeName = "numeric")]
        [ForeignKey("RIC_EmployeeRef")]
        public int? RE_Mgr_ID { get; set; }

        [NotMapped]
        [Column(TypeName = "numeric")]
        [Display(Name = "Organization Id")]
        public int RE_OrgID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Employee Name")]
        public string RE_Employee_Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Jobdiva User Name")]
        public string RE_Jobdiva_User_Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Alternate Jobdiva User Name")]
        public string RE_AltJobdiva_User_Name { get; set; }

        [StringLength(50)]
        [Display(Name = "Shoretel Name")]
        public string RE_Shortel_Name { get; set; }



        [Required]
        [MinLength(8)]
        [StringLength(20)]
        [Display(Name = "Password")]
        public string RE_Password { get; set; }

        [NotMapped]
        [Column(TypeName = "numeric")]
        public int RE_DeptID { get; set; }

        [StringLength(12)]
        [Display(Name = "Contact Number")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid Phone number")]
        public string RE_Contact_Num { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Email ID")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string RE_Email { get; set; }

        [StringLength(30)]
        [Display(Name = "Create User")]
        public string RE_Create_User { get; set; }

        [StringLength(30)]
        [Display(Name = "Exp. Before Joining Sunrise")]
        public string RE_Experience { get; set; }

        [Column(TypeName = "date")]
        [Display(Name = "Joining Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? RE_Joining_Date { get; set; }

        //new properties added 06-03-2018.

        [Display(Name = "Resignation date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? RE_Resign_Date { get; set; }

        [Required]
        [Display(Name = "AKA Name")]
        [StringLength(50)]
        public string RE_AKA_Name { get; set; }


        //--------------------------------------

        [Display(Name = "Created Date")]
        public DateTime RE_Create_Dt { get; set; }

        [StringLength(30)]
        [Display(Name = "Update User")]
        public string RE_Upd_User { get; set; }
        [Display(Name = "Update Date")]
        public DateTime? RE_Upd_Dt { get; set; }

        [Required]
        [Display(Name = "Experience")]
        public Boolean RE_Exp { get; set; }


        [Display(Name = "User Status")]
        public Boolean RE_User_Status { get; set; }



        public string RE_ReviewerList { get; set; }


        public int? RE_AnnualFeedBackFormID { get; set; }

        // not mapped to database...
        [NotMapped]
        [Range(1, int.MaxValue, ErrorMessage = "The value must be greater than 0")]
        [Required(ErrorMessage = "Please select the role")]
        public int RoleID { get; set; }
        [NotMapped]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("RE_Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
        [NotMapped]
        [Display(Name = "Role")]
        public IEnumerable<SelectListItem> RoleList { get; set; }

        [NotMapped]
        [Display(Name = "Designation")]
        public IEnumerable<SelectListItem> DesignationList { get; set; }

        [NotMapped]
        [Display(Name = "Department")]
        public IEnumerable<SelectListItem> DepartmentList { get; set; }

        [NotMapped]
        [Display(Name = "Reporting Manager")]
        public IEnumerable<SelectListItem> ManagerList { get; set; }

        [NotMapped]
        [Display(Name = "")]
        public IEnumerable<SelectListItem> SubmissionRule { get; set; }


        [NotMapped]
        [Display(Name = "Reporting Manager")]
        public int ManagerName { get; set; }



        [NotMapped]
        [Display(Name = "Reporting To")]
        public string ReportingTo { get; set; }
        [NotMapped]
        //  [Display(Name = "Reporting To")]
        public string MgrCD { get; set; }

        [NotMapped]
        // [Required]
        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? RE_Start_Date { get; set; }

        [NotMapped]
        // [Required]
        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? RE_End_Date { get; set; }

        //----------------------------------------------------------

        // Navigation properties 
        public virtual ICollection<RIC_User_Role> RIC_User_Role { get; set; }

        // public virtual ICollection<RIC_Reviewer> RIC_Reviewer { get; set; }

        //submission rule navagation property.
        public virtual RIC_SubmissionRule RIC_SubmissionRule { get; set; }

        public virtual ICollection<RIC_Employee> RIC_EmployeeNav { get; set; }
        public virtual RIC_Employee RIC_EmployeeRef { get; set; }

        [ForeignKey("RMS_Designation")]
        public int? RE_DesignationID { get; set; }

         [ForeignKey("RMS_Department")]
        public int ? RE_DepartmentID { get; set; }



        public virtual RMS_Department RMS_Department { get; set; }

        public virtual RMS_Designation RMS_Designation { get; set; }



       public virtual ICollection<RIC_ClientMapping> RIC_ClientMapping { get; set; }
    }
}
