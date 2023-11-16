using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class AccessLogList
    {
        public List<AccessLogResult> AccessLogResults { get; set; }
        public List<TeamMatrixResult> TeamMatrixResult { get; set; }
    }
}
