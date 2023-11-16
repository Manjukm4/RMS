using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{

   public partial class RIC_Role
    {
       public RIC_Role()
       {
           this.RIC_User_Role = new List<RIC_User_Role>();
       }

        [Column(TypeName = "numeric")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RR_RoleId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Role Name")]
        public string RR_Role_Name { get; set; }

        [Column(TypeName = "numeric")]
        [Display(Name = "Organization ID")]
        public int RR_OrgID { get; set; }

        [StringLength(1)]
        public string RR_Role_Status { get; set; }

        [StringLength(50)]
        [Display(Name = "Create User")]
        public string RR_Create_User { get; set; }

        public DateTime RR_Create_Dt { get; set; }

        [StringLength(30)]
        [Display(Name = "Update User")]
        public string RR_Upd_User { get; set; }

        public DateTime? RR_Upd_Dt { get; set; }

        // Navigation properties 
        public virtual ICollection<RIC_User_Role> RIC_User_Role { get; set; }



    }
}
