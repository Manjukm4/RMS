using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIC.Models.Dashboard
{
  
    public class AddPanelsPopup
    {
        public int Id { get; set; }

        public string PanelName { get; set; }
        
        public string ImgUrl { get; set; }

        public string ActionName { get; set; }

        public string ControllerName { get; set; }

        public int SortOrder { get; set; }

        public int ColumnNumber { get; set; }

        public int ColumnWidth { get; set; }

        public bool PanelSelected { get; set; }
    }
}