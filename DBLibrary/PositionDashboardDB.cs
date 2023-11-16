using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
  public class PositionDashboardDB
    {
        public int Year { get; set; }
        public int Q1Pos { get; set; }
        public int Q2Pos { get; set; }
        public int Q3Pos { get; set; }
        public int Q4Pos { get; set; }
        public int Q1Submissions { get; set; }
        public int Q2Submissions { get; set; }
        public int Q3Submissions { get; set; }
        public int Q4Submissions { get; set; }
        public int TotalSubmissions { get; set; }

        public int Q1Interview { get; set; }
        public int Q2Interview { get; set; }
        public int Q3Interview { get; set; }
        public int Q4Interview { get; set; }
        public int TotalInterview { get; set; }

        public int Q1Hire { get; set; }
        public int Q2Hire { get; set; }
        public int Q3Hire { get; set; }
        public int Q4Hire { get; set; }
        public int TotalHire { get; set; }

        public int Total { get; set; }
        public string Position_Type { get; set; }
    }
}
