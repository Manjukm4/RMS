using DBLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Client
{
    public class ClientListView
    {

        public bool IsDisable { get; set; }
        public List<RIC_Client> clientList { get; set; }
    }
}