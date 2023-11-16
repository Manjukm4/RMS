using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    public  class RMS_ViewsConfig
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RV_Id { get; set; }

        [StringLength(50)]
        public string RV_ViewName { get; set; }

        [StringLength(50)]
        public string RV_ColumnName { get; set; }

        [StringLength(20)]
        public string RV_ColumnType { get; set; }

        [StringLength(20)]
        public string RV_ControlType { get; set; }

        [StringLength(50)]
        public string Lk_ViewName { get; set; }

        [StringLength(50)]
        public string LK_ColumnName { get; set; }

        [StringLength(50)]
        public string RV_DisplayName { get; set; }

        [StringLength(500)]
        public string RV_Description { get; set; }
    }
}
