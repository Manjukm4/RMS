using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DBLibrary.UnitOfWork;

namespace RIC.Utility
{
    public class ReviewUtitity
    {
        string mgrRoleName = System.Configuration.ConfigurationManager.AppSettings["ManagerRole"];
        string tlRoleName = System.Configuration.ConfigurationManager.AppSettings["TLRole"];
        string empRoleName = System.Configuration.ConfigurationManager.AppSettings["EmployeeRole"];
        string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        int adminRoleId = int.Parse(System.Configuration.ConfigurationManager.AppSettings["AdminRoleID"]);
        string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
        string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
        string devRoleName = System.Configuration.ConfigurationManager.AppSettings["DEVRole"];
        string devLeadRole = System.Configuration.ConfigurationManager.AppSettings["DEV_LeadRole"];
        string stafingDirectorRole = System.Configuration.ConfigurationManager.AppSettings["StaffingDirector"];
        private UnitOfWork unitOfWork = new UnitOfWork();
        public List<SelectListItem> GetReviewerList()
        {
            //get the user list excluding roles.
            var userRoles = new List<string> {HrRoleName,empRoleName,devRoleName,AdminRoleName};
            var reviewerList = unitOfWork.User
                .Get(s => !userRoles.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name)
                 &&s.RE_Resign_Date==null)
                .Select(em => new SelectListItem
                {
                    Text = em.RE_Jobdiva_User_Name,
                    Value = em.RE_Emp_Cd

                }).OrderBy(o=>o.Text).ToList();
            return reviewerList;
        }
        public List<SelectListItem> GetDirectorList()
        {
             var userRoles = new List<string> {directorRoleName,stafingDirectorRole};
            var reviewerList = unitOfWork.User
                .Get(s => userRoles.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name) 
                    && s.RE_Resign_Date==null)
                .Select(em => new SelectListItem
                {
                    Text = em.RE_Jobdiva_User_Name,
                    Value = em.RE_Emp_Cd
                }).OrderBy(o => o.Text).ToList();
            return reviewerList;
        }
        //public List<SelectListItem> GetTlList1()
        //{
        //    var reviewerList = unitOfWork.User
        //        .Get(s => s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == tlRoleName
        //         && s.RE_Resign_Date == null)
        //        .Select(em => new SelectListItem
        //        {
        //            Text = em.RE_Jobdiva_User_Name,
        //            Value = em.RE_Emp_Cd
        //        }).OrderBy(o => o.Text).ToList();
        //    return reviewerList;
        //}
        public List<SelectListItem> GetHrLsit()
        {
            var reviewerList = unitOfWork.User
                .Get(s => s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == HrRoleName
                 && s.RE_Resign_Date == null)
                .Select(em => new SelectListItem
                {
                    Text = em.RE_Jobdiva_User_Name,
                    Value = em.RE_Emp_Cd
                }).OrderBy(o => o.Text).ToList();
            return reviewerList;
        }
    }
}