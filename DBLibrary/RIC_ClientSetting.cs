using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_ClientSetting
    {
        [Key]
        public int RC_Id { get; set; }

        [StringLength(50)]
        public string RC_EmpCd { get; set; }
        
        public string RC_ClientList { get; set; }

    }
}
