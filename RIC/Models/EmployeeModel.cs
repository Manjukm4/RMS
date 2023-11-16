using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models
{
    public class EmployeeModel
    {
        [Column(TypeName = "numeric")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RE_EmpId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Employee ID")]
        public string RE_Emp_Cd { get; set; }

        [Column(TypeName = "numeric")]
        [ForeignKey("RIC_EmployeeRef")]
        public int? RE_Mgr_ID { get; set; }

        [Column(TypeName = "numeric")]
        [Display(Name = "Organization Id")]
        public int RE_OrgID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string RE_First_Name { get; set; }

        //[Required]
        //[StringLength(50)]
        //[Display(Name = "Last Name")]
        //public string RE_Last_Name { get; set; }

        [Required]
        [StringLength(5)]
        [Display(Name = "Password")]
        public string RE_Password { get; set; }

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

        [Column(TypeName = "date")]
        [Display(Name = "Joining Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime? RE_Joining_Date { get; set; }
        
        [Display(Name = "Created Date")]
        public DateTime RE_Create_Dt { get; set; }

        [StringLength(30)]
        [Display(Name = "Update User")]
        public string RE_Upd_User { get; set; }
        
        [Display(Name = "Update Date")]
        public DateTime? RE_Upd_Dt { get; set; }

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
        [Display(Name = "Manager")]
        public IEnumerable<SelectListItem> ManagerList { get; set; }
    }
}