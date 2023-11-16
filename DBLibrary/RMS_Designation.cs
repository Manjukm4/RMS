using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RMS_Designation
    {
        [Key]
        public int RD_ID { get; set; }

        public string RD_Designation { get; set; }

        public virtual ICollection<RIC_Employee> RIC_Employee { get; set; }
    }
}
