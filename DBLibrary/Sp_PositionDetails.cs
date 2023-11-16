using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
     public class Sp_PositionDetails
    {

        public string Company { get; set; }
        public string JobDiva_Ref_Num { get; set; }
        public string Optional_Ref_Num { get; set; }
        public int submission { get; set; }
        public int Interview { get; set; }
        public int Hire { get; set; }
        public DateTime Issued_Date { get; set; }
        public DateTime? Submission_Date { get; set; }
        public DateTime? Interview_Date { get; set; }
        public DateTime? Hire_Date { get; set; }

         
    }
}
