using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary
{
	public class GetDashboardMatricsResult
	{
		public DateTime DM_Date { get; set; }

		public int DM_Week { get; set; }

		public int? DM_Prod_Recruiters { get; set; }

		public int? DM_Submissions { get; set; }
		public string DM_SubmissionsToolTipData { get; set; }

		public int? DM_Interviews { get; set; }
		public string DM_InterviewsToolTipData { get; set; }

		public int? DM_Hires { get; set; }
		public string DM_HiresToolTipData { get; set; }

		public int? DM_CallConnected_In { get; set; }

		public int? DM_VoiceMessages_In { get; set; }

		public double? DM_TotalDuration_In { get; set; }

		public int? DM_Call_Connected_Out { get; set; }

		public int? DM_Voice_Messages_Out { get; set; }

		public double? DM_Total_Duration_Out { get; set; }

		public int? DM_CallConnected { get; set; }

		public double? DM_TotalDuration { get; set; }

		public int? DM_Prod_RecruitersCalls { get; set; }

		public int? DM_TotalCalls { get; set; }
	}
}
