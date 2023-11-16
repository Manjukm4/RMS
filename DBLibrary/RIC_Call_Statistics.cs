using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
    //Added by Madhu 23-03-2018
    public class RIC_Call_Statistics
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RC_Id { get; set; }

       [Index("IX_RC_Composite", 1, IsUnique = true)]
        [Required]
        [StringLength(50)]
        public string RC_Emp_Cd { get; set; }

        [Index("IX_RC_Composite", 2, IsUnique = true)]
        [Required]
        public DateTime RC_Date { get; set; }

        [Index("IX_RC_Composite", 3, IsUnique = true)]
        [Required]
        public TimeSpan RC_Time { get; set; }

        [Index("IX_RC_Composite", 4, IsUnique = true)]
        [Required]
        [StringLength(5)]
        public string RC_CallType { get; set; }

        [Index("IX_RC_Composite", 5, IsUnique = true)]
        [Required]
        [StringLength(20)]
        public string RC_Dailed { get; set; }

        [Index("IX_RC_Composite", 6, IsUnique = true)]
        [Required]
        [StringLength(20)]
        public string RC_Calling { get; set; }

        [Required]
        public TimeSpan RC_Duration { get; set; }

        public int RC_Call_Connected { get; set; }
        
        public int RC_Voice_Message { get; set; }

        [StringLength(20)]
        public string RC_PRI { get; set; }
        public DateTime RC_UploadedDate { get; set; }
        public string RC_Source { get; set; }
    }
}
