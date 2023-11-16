using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RIC.Models;
using DBLibrary;
using DBLibrary.UnitOfWork;
namespace RIC.Utility
{
    public class IncentiveIndexAction
    {
       public UnitOfWork unitOfwork = new UnitOfWork();
        public List<IncentiveViewModel> getIncentivesForUser(string empCd)
        {
            List<IncentiveViewModel> incetiveView = new List<IncentiveViewModel>();
            DateTime datetoday=SystemClock.US_Date.Date;
            var incentiveList = unitOfwork.Incentive.Get(s => s.RI_EmpCd == empCd).ToList();

            var jandata = incentiveList.Where(s => s.RI_StartDate.Month == 1);
            var febdata = incentiveList.Where(s => s.RI_StartDate.Month == 2);

            IncentiveViewModel JanIns = new IncentiveViewModel();
            if (jandata.Count() != 0)
            {
                JanIns.Month = jandata.FirstOrDefault().RI_StartDate;
                JanIns.TotalIncentive = jandata.Where(s => s.RI_Jan_Incentive != null).Sum(s => s.RI_Jan_Incentive.Value);
                JanIns.TotalStart = jandata.Count();
                JanIns.TotalMargin = jandata.Where(s => s.RI_Jan_Incentive != null).Sum(s => s.RI_NetMargin);
            }
            else
            {
                JanIns.Month = new DateTime(2018,1,1);
                JanIns.TotalIncentive = 0;
                JanIns.TotalStart = 0;
                JanIns.TotalMargin = 0;

            }


            IncentiveViewModel febIns = new IncentiveViewModel();
            if (febdata.Count() != 0)
            {
                febIns.Month = febdata.FirstOrDefault().RI_StartDate;
                febIns.TotalIncentive = febdata.Where(s => s.RI_Feb_Incentive != null).Sum(s => s.RI_Feb_Incentive.Value);
                febIns.TotalStart = febdata.Count();
                febIns.TotalMargin = febdata.Where(s => s.RI_Feb_Incentive != null).Sum(s => s.RI_NetMargin);
            }
            else
            {
                JanIns.Month = new DateTime(2018, 2,1);
                JanIns.TotalIncentive = 0;
                JanIns.TotalStart = 0;
                JanIns.TotalMargin = 0;
            }
                
                //(from incentive in unitOfwork.Incentive.Get()
                //                where incentive.RI_EmpCd == empCd
                //                select incentive
                //                ).ToList(); 
            
           // incetiveView=           
            incetiveView.Add(febIns);
            incetiveView.Add(JanIns);
            return incetiveView;
        }


    }
}