using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class RIC_User_Role
    {

      


        [Column(TypeName = "numeric")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RUR_Id { get; set; }

        [Column(TypeName = "numeric")]
        [ForeignKey("RIC_Employee")]
        public int RUR_Emp_ID { get; set; }

        [Column(TypeName = "numeric")]
        [ForeignKey("RIC_Role")]
        public int RUR_Role_ID { get; set; }

        public int RD_OrgID { get; set; }

        [StringLength(30)]
        public string RD_Create_User { get; set; }

        public DateTime RD_Create_Dt { get; set; }

        [StringLength(30)]
        public string RD_Upd_User { get; set; }

        public DateTime? RD_Upd_Dt { get; set; }

        // Navigation properties 
        public virtual RIC_Employee RIC_Employee { get; set; }
        public virtual RIC_Role RIC_Role { get; set; }


    }
}
