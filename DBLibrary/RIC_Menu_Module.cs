using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_Menu_Module
    {
        [Key]
        public int RM_MenuID { get; set; }

        [Display(Name="Parent Menu")]
        public int RM_MenuParentId { get; set; }

        [Display(Name="Menu Name")]
        [StringLength(20)]
        public string RM_MenuName { get; set; }

        [Display(Name="Controller")]
        [StringLength(20)]
        public string RM_ControllerName { get; set; }

        [Display(Name="Action")]
        [StringLength(20)]
        public string RM_ActionName { get; set; }

        [Display(Name="Sort Order")]
        public int RM_Sort_Order { get; set; }

        [Display(Name="Menu Item Level")]
        public int RM_Menuitem_Level { get; set; }

        [Display(Name="Roles")]
        [StringLength(100)]
        public string RM_Roles { get; set; }
    }
}
