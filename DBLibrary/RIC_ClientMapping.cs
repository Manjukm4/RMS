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
   public class RIC_ClientMapping
    {
       [Key]
       public int RCM_Id { get; set; }     

       [Display(Name = "Created Date")]
       public DateTime? RCM_CreatedDate { get; set; }

       [Display(Name = "Updated Date")]
       public DateTime? RCM_UpdatedDate { get; set; }

       [Display(Name = "Created By")]
       public string RCM_CreatedBy { get; set; }

       [Display(Name = "Updated By")]
       public string RCM_UpdatedBy { get; set; }

       [Display(Name = "Team Lead")]
       public string RCM_TeamLeadId { get; set; }

       [Display(Name = "Effective Start Date")]
       public DateTime RCM_StartDate{ get; set; }

       [Display(Name = "Effective End Date")]
       public DateTime? RCM_EndDate { get; set; }


        [NotMapped]
       [Display(Name = "Client List")]
       public IEnumerable<SelectListItem> ClientList { get; set; }

       public IEnumerable<SelectListItem> ClientList2 { get; set; }


       [ForeignKey("RIC_Client")]
       public int RCM_ClientId { get; set; }


       [ForeignKey("RIC_Employee")]
       public int RCM_EmpId { get; set; }


       public virtual RIC_Client RIC_Client { get; set; }
       public virtual RIC_Employee RIC_Employee { get; set; }

    }
}
