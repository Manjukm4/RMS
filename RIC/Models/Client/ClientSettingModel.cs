using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RIC.Models.Client
{
    public class ClientSettingModel
    {

        public IEnumerable<string> ClientList { get; set; }

        public IEnumerable<string> SelectedClient { get; set; }
    }
}