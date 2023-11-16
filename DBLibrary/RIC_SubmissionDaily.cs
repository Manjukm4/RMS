using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_SubmissionDaily
    {
        [Key]
        public int RS_Sub_ID { get; set; }

        [Index("IX_RC_SubmissionComposite", 1, IsUnique = true)]
        [Required]
        [StringLength(50)]
        [Display(Name="Employee ID")]
        public string RS_Emp_Cd { get; set;}

        [Index("IX_RC_SubmissionComposite", 2, IsUnique = true)]
        [Required]
        [Display(Name="Date")]
        public DateTime RS_Date { get; set; }

        [Required]
        [Display(Name = "Submissions")]
        public int RS_Submissions { get; set; }

        [Required]
        [Display(Name = "Interviews")]
        public int RS_Interviews { get; set; }

        [Required]
        [Display(Name = "Hires")]
        public int RS_Hires { get; set;}

        [NotMapped]
        [Display(Name = "Jobdiva User Name ")]
        public string RE_Jobdiva_User_Name { get; set; }
    }
}
