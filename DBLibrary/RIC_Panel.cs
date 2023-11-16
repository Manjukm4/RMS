using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public class RIC_Panel
    {
        [Key]
        public int RP_Id { get; set; }
        
        [StringLength(50)]
        public string RP_PanelName { get; set; }
        
        [StringLength(50)]
        public string RP_ActionName { get; set; }
        
        [StringLength(50)]
        public string RP_ConTrollerName { get; set; }
        
        [StringLength(50)]
        public string RP_ImageName { get; set; }

        public ICollection<RIC_UserDashboardPanel> RIC_UserDashboardPanel { get; set; }
    }
}
