using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [StringLength(20)]
        [Display(Name = "Old Password")]
        public string OldPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [MinLength(8)]
        [StringLength(20)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [System.ComponentModel.DataAnnotations.Compare("NewPassword", ErrorMessage = "Confirm password doesn't match, Type again !")]      
        public string ConfirmPassword { get; set; }


        public string OldPasswordCompare { get; set; }
        public string EmpCD { get; set; }
    }
}