using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RIC.Models
{
    public class UserRoleModel
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

        [StringLength(30)]
        public string RD_Upd_User { get; set; }
    }
}