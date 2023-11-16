using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace RIC.Models.Account
{
    public class AddClientList
    {
        public int Id { get; set; }

        public string ClientId { get; set; }

        [Display(Name = "Client")]
        public string Client { get; set; }

    }
}