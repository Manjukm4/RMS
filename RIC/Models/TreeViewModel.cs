using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DBLibrary;
namespace RIC.Models
{
    public class TreeViewModel
    {
       // public RIC_Employee MgrName { get; set; }
        //public List<RIC_Employee> Employees { get; set; }
        public int TotalEmoloyee { get; set; }
        public string MgrName { get; set; }
        public List<Employees> Employees { get; set; }
    }
    public class Employees
    {
        public string EmpCd { get; set; }
        public string JobdivaUserName { get; set; }

    }

}