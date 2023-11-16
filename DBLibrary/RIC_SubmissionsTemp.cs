using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_SubmissionsTemp
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string EmpCd { get; set; }
        [Required]
        public string EmpName { get; set; }
        [Required]
        public DateTime StrtDate { get; set; }
        public DateTime ? EndDate { get; set; }
        public string CompanyName { get; set; }
    }
}
