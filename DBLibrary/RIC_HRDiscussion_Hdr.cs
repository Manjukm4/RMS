using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_HRDiscussion_Hdr
    {
        [Key]
        public int RH_ID { get; set; }

        [Required]
        [StringLength(50)]
        public string RH_EmployeeID { get; set; }

        [Required]
        [StringLength(50)]
        public string RH_ReviewerID { get; set; }
        
        public DateTime? RH_ReviewDate { get; set; }

        public string RH_AgendaType { get; set; }

        [Required]
        [StringLength(50)]
        public string RH_Status { get; set; }

        public string RH_CreatedBy { get; set; }

        public DateTime? RH_CreatedDate { get; set; }

        public string RH_UpdatedBy { get; set; }

        public DateTime? RH_UpdatedDate { get; set; }

        public virtual ICollection<RIC_HRDiscussion_Dtl> RIC_HRDiscussion_Dtl { get; set; }


    }
}
