using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_Client
    {
        [Key]
        public int RC_Id { get; set; }

        [StringLength(255)]
        [Display(Name = "Client Name")]
        public string RC_ClientName { get; set; }

        [Display(Name = "Created Date")]
        public DateTime RC_CreatedDate { get; set; }

        [Display(Name = "Created By")]
        public string RC_CreatedBy { get; set; }

     
        public virtual ICollection<RIC_ClientMapping> RIC_ClientMapping { get; set; }
    }
}
