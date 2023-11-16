using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace DBLibrary
{
    public class RMS_Department
    {
        [Key]
        public int RD_ID { get; set; }

        public string RD_Department { get; set; }

        public virtual ICollection<RIC_Employee> RIC_Employee { get; set; }
    }
}
