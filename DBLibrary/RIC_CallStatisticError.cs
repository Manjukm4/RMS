using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DBLibrary
{
  public  class RIC_CallStatisticError
    {
        //[Key]
        //public int RE_id { get; set; }
        [Key]
        public string RE_Emp_Cd { get; set; }
        public string PRI { get; set; }
        public string DateString { get; set; }
        public string TimeString { get; set; }
        public string CallType { get; set; }
        public string Dialed { get; set; }
        public string Calling { get; set; }
        public string ShName { get; set; }
        public string CallDurationString { get; set; }
        public int ErrorCode { get; set; }
        public int ErrorColumn { get; set; }
        
       
    }
}
