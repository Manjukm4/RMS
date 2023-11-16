using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class RIC_UserDashboardPanel
    {
       [Key]
       public int RU_Id { get; set; }
       
       [StringLength(50)]
       public string RU_EmpCd { get; set; }

       [ForeignKey("RIC_Panel")]
       public int RU_PanelId { get; set; }

       public int RU_ColumnNumber { get; set; }

       public int RU_ColumnWidth { get; set; }

       public int RU_SortOrder { get; set; }

       public RIC_Panel RIC_Panel { get; set; }


    }
}
