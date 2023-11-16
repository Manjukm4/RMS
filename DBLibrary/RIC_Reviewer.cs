using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class RIC_Reviewer
    {
       [Key]
       public int RR_ID { get; set; }      

       [Required]
       [StringLength(50)]
       public string RR_EmpCd { get; set; }

       [Required]
       [StringLength(50)]
       public string RR_ReviewerCd { get; set; }

       // Navigation properties 
      // public virtual RIC_Employee RIC_Employee { get; set; }
    }
}
