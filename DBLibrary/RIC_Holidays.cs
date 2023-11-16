using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_Holidays
    {
        [Key]
        public int RH_Id { get; set; }
        
        [Required]
        [Display(Name="Date")]
        public DateTime RH_Date { get; set; }

        [Display(Name="Festival/Event")]
        public string RH_Festival { get; set; }
    }
}
