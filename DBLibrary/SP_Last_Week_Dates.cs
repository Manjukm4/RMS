using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class SP_Last_Week_Dates
    {
        public DateTime Start_week1 { get; set; }
        public DateTime end_week1 { get; set; }
        public DateTime Start_week2 { get; set; }
        public DateTime end_week2 { get; set; }
        public DateTime Start_week3 { get; set; }
        public DateTime end_week3 { get; set; }
        public DateTime Cur_wk_st { get; set; }
        public DateTime Cur_wk_end { get; set; }
    }
}
