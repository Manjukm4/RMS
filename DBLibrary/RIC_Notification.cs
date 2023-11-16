using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
   public class RIC_Notification
    {
        [Key]
        public int RN_ID { get; set; }

        [StringLength(50)]
        public string RN_EmpCd { get; set; }

        public int Rn_ReviewID { get; set; }

       [StringLength(100)]
        public string RN_NotificationType { get; set; }

        [StringLength(100)]
        public string RN_NotificationText { get; set; }

        public bool RN_IsSeen { get; set; }

        public bool RN_DirSeen { get; set; }

        public bool RN_HrSeen { get; set; }

        public DateTime Date { get; set; }


    }
}
