using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data.Entity;
using System.Data;

namespace DBLibrary.Repository
{
    public class EmployeeRepository<TEntity> : GenericRepositiory<TEntity> where TEntity : RIC_Employee
    {
        string AdminRoleName = System.Configuration.ConfigurationManager.AppSettings["AdminRole"];
        string EmployeeRoleName = System.Configuration.ConfigurationManager.AppSettings["EmployeeRole"];
        string directorRoleName = System.Configuration.ConfigurationManager.AppSettings["DirectorRole"];
        string AccMgrRoleName = System.Configuration.ConfigurationManager.AppSettings["AccountingManagerRole"];
        string mgrRoleName = System.Configuration.ConfigurationManager.AppSettings["ManagerRole"];
        string tlRoleName = System.Configuration.ConfigurationManager.AppSettings["TLRole"];
        string devTlRoleName = System.Configuration.ConfigurationManager.AppSettings["DEV_LeadRole"];
        string HrRoleName = System.Configuration.ConfigurationManager.AppSettings["HRRole"];
        string devRoleName = System.Configuration.ConfigurationManager.AppSettings["DEVRole"];
        public EmployeeRepository(RIC_DBEntities context)
            : base(context) // call the base constructor
        {
        }

        public override void Insert(TEntity entity)
        {
            entity.RE_Create_Dt = SystemClock.US_Date;
            var uRole = new RIC_User_Role
            {
                RUR_Role_ID = entity.RoleID,
                RD_Create_Dt = SystemClock.US_Date,
                RD_OrgID = entity.RE_OrgID
            };
            entity.RIC_User_Role.Add(uRole);
            base.Insert(entity);
        }

        public TEntity GetByEmpID(string empID)
        {
            return dbSet.AsNoTracking().FirstOrDefault(s => s.RE_Emp_Cd == empID);
        }

        public List<SelectListItem> getMgrList(int roleID = 0)
        {
            int mgrRoleID = int.Parse(System.Configuration.ConfigurationManager.AppSettings["ManagerRoleID"]);
            // int tlRoleID = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TLRoleID"]);
          

            var listOfRole = new List<string>() { mgrRoleName, tlRoleName, devTlRoleName };

            // bind the manager list.
            List<SelectListItem> _mgrSelectedItem = new List<SelectListItem>();

            var managerList = dbSet.Where(s => listOfRole.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name))
                 .OrderBy(s => s.RE_Jobdiva_User_Name);

            foreach (var mgr in managerList)// add the items in select list.
            {
                SelectListItem selectListItem = new SelectListItem();
                if (mgr.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == tlRoleName
                    || mgr.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name == devTlRoleName)
                    if (roleID == mgrRoleID)
                        continue;
                    else
                        selectListItem.Text = mgr.RE_Jobdiva_User_Name;
                else
                    selectListItem.Text = mgr.RE_Jobdiva_User_Name;
                selectListItem.Value = mgr.RE_EmpId.ToString();
                _mgrSelectedItem.Add(selectListItem);
            }       
            return _mgrSelectedItem;
        }

        public List<SelectListItem> getMgrList()
        {
            List<SelectListItem> _mgrSelectedItem = new List<SelectListItem>();
            var listOfRole = new List<string>() {devRoleName,EmployeeRoleName,AdminRoleName
                                                  ,directorRoleName,AccMgrRoleName,HrRoleName};
            _mgrSelectedItem = dbSet.Where(s => !listOfRole.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name))
                .OrderBy(s => s.RE_Jobdiva_User_Name)
                .Select(s=> new SelectListItem
                {
                    Text=s.RE_Jobdiva_User_Name,
                    Value=s.RE_Emp_Cd
                }).ToList();
            _mgrSelectedItem.Add(new SelectListItem { Text = "Directors", Value = "Directors" });
            return _mgrSelectedItem;
        }

        public List<SelectListItem> getMgrList1()
        {
            List<SelectListItem> _mgrSelectedItem = new List<SelectListItem>();
            var listOfRole = new List<string>() {devRoleName,EmployeeRoleName,AdminRoleName
                                                  ,directorRoleName,AccMgrRoleName,HrRoleName};
            _mgrSelectedItem = dbSet.Where(s => !listOfRole.Contains(s.RIC_User_Role.FirstOrDefault().RIC_Role.RR_Role_Name))
                .OrderBy(s => s.RE_Jobdiva_User_Name)
                .Select(s => new SelectListItem
                {
                    Text = s.RE_Jobdiva_User_Name,
                    Value = s.RE_Jobdiva_User_Name
                }).ToList();
            _mgrSelectedItem.Add(new SelectListItem { Text = "Directors", Value = "Directors" });
            return _mgrSelectedItem;
        }

        public void resetPassword(string empID, string newPassword)
        {
            ////get the user details from db.
            var user = dbSet.FirstOrDefault(s => s.RE_Emp_Cd == empID);
            user.RE_Password = newPassword;
            user.ConfirmPassword = newPassword;
            user.RoleID = 1;
            base.Update(user);
        }

        public override void Update(TEntity entityToUpdate)
        {
            ////get the user details from db.
            var user = dbSet.FirstOrDefault(s => s.RE_EmpId == entityToUpdate.RE_EmpId);
            // update reporting manager history.
            if (user.RE_Mgr_ID != entityToUpdate.RE_Mgr_ID)
            {
                context.RIC_ReportingHistory.Add(new RIC_ReportingHistory()
                {
                    RR_EmpCD = user.RE_Emp_Cd,
                    RR_MgrCD = user.RIC_EmployeeRef.RE_Emp_Cd,
                    RR_FromDate = user.RE_Start_Date,
                    RR_ToDate = user.RE_End_Date,
                    RR_CreatedDate = SystemClock.US_Date
                });
                //context.SaveChanges();
            }

            user.RE_Shortel_Name = entityToUpdate.RE_Shortel_Name;
            user.RE_Sub_Rule_ID = entityToUpdate.RE_Sub_Rule_ID;
            //set the properties.
            // user.ConfirmPassword =null;
            user.ConfirmPassword = entityToUpdate.ConfirmPassword;
            user.RE_Contact_Num = entityToUpdate.RE_Contact_Num;
            user.RE_DeptID = 0; //entityToUpdate.RE_DeptID;
            user.RE_Email = entityToUpdate.RE_Email;
            user.RE_Emp_Cd = entityToUpdate.RE_Emp_Cd;
            user.RE_Employee_Name = entityToUpdate.RE_Employee_Name;
            // user.RE_Last_Name = entityToUpdate.RE_Last_Name;
            user.RE_Start_Date = entityToUpdate.RE_Start_Date;
            user.RE_End_Date = entityToUpdate.RE_End_Date;
            user.RE_Mgr_ID = entityToUpdate.RE_Mgr_ID;
            user.RE_Exp = entityToUpdate.RE_Exp;
            user.RE_Resign_Date = entityToUpdate.RE_Resign_Date;
            user.RE_AKA_Name = entityToUpdate.RE_AKA_Name;
            user.RE_User_Status = entityToUpdate.RE_User_Status;
            //user.RE_Password = entityToUpdate.RE_Password;           
            user.RE_Upd_Dt = SystemClock.US_Date;
            user.RoleID = entityToUpdate.RoleID;
            user.RoleList = entityToUpdate.RoleList;
            user.RE_Joining_Date = entityToUpdate.RE_Joining_Date;
            user.RIC_User_Role.First().RUR_Role_ID = entityToUpdate.RoleID;
            user.RIC_User_Role.First().RD_Upd_Dt = SystemClock.US_Date;
            user.RIC_User_Role.First().RD_OrgID = 0;//entityToUpdate.RE_OrgID;

            user.RE_ReviewerList = entityToUpdate.RE_ReviewerList;
            user.RE_Jobdiva_User_Name = entityToUpdate.RE_Jobdiva_User_Name;
            user.RE_AltJobdiva_User_Name = entityToUpdate.RE_AltJobdiva_User_Name;
			user.RE_DesignationID = entityToUpdate.RE_DesignationID;
            user.RE_DepartmentID = entityToUpdate.RE_DepartmentID;
            user.RE_Experience = entityToUpdate.RE_Experience;
            user.RE_Upd_User = entityToUpdate.RE_Upd_User;
            base.Update(user);
        }
        //get the user list with current 
        public IEnumerable<RIC_Employee> getAllUsers()
        {
            IEnumerable<RIC_Employee> listOfEmployee = new List<RIC_Employee>();
            DateTime dtToday = SystemClock.US_Date.Date;
            // get the employees and manager from database.
            listOfEmployee = (from employee in
                                  (from employee in context.RIC_Employee
                                   join reportingHistory in context.RIC_ReportingHistory.
											  Where(s => (s.RR_ToDate == null ? dtToday : s.RR_ToDate) >= dtToday && s.RR_FromDate <= dtToday)
                                   on employee.RE_Emp_Cd equals reportingHistory.RR_EmpCD
                                   into rightJoin
                                   from reportingHistory in rightJoin.DefaultIfEmpty() // right join                                
                                   select new
                                   {
                                       empID = employee.RE_EmpId,
                                       EmpCd = employee.RE_Emp_Cd,
                                       EmpName = employee.RE_Employee_Name,
                                       RE_Jobdiva_User_Name = employee.RE_Jobdiva_User_Name,
                                       MgrCD = reportingHistory.RR_MgrCD,
                                       FromDate = reportingHistory.RR_FromDate,
                                       ToDate = reportingHistory.RR_ToDate,
                                       userRole = employee.RIC_User_Role,
                                       resignDate = employee.RE_Resign_Date,
                                       subRuleID = employee.RE_Sub_Rule_ID,
                                       SubmissionRule = employee.RIC_SubmissionRule,
                                       AnnualFeedBackFormID = employee.RE_AnnualFeedBackFormID,
                                       Department = employee.RMS_Department
                                   })
                              join mgrTable in context.RIC_Employee
                             on employee.MgrCD equals mgrTable.RE_Emp_Cd
                             into leftjoin
                              from mgrTable in leftjoin.DefaultIfEmpty() // left join    
                              select new 
                              {
                                  RE_EmpId = employee.empID,
                                  RE_Emp_Cd = employee.EmpCd,
                                  RE_Employee_Name = employee.EmpName,
                                  RE_Jobdiva_User_Name = employee.RE_Jobdiva_User_Name,
                                  ReportingTo = employee.MgrCD == "Directors" ? "Directors": mgrTable.RE_Jobdiva_User_Name,
                                  RIC_User_Role = employee.userRole,
                                  RE_Start_Date = employee.FromDate,
                                  RE_End_Date = employee.ToDate,
                                  MgrCD = employee.MgrCD,
                                  resignDate = employee.resignDate,
                                  subRuleID = employee.subRuleID,
                                  subRule = employee.SubmissionRule,
                                  RE_AnnualFeedBackFormID = employee.AnnualFeedBackFormID,
                                  Department = employee.Department
                              }).ToList().Select(s => new RIC_Employee()
                              {

                                  RE_EmpId = s.RE_EmpId,
                                  RE_Emp_Cd = s.RE_Emp_Cd,
                                  RE_Employee_Name = s.RE_Employee_Name,
                                  RE_Jobdiva_User_Name = s.RE_Jobdiva_User_Name,
                                  ReportingTo = s.ReportingTo,
                                  RIC_User_Role = s.RIC_User_Role,
                                  RE_Start_Date = s.RE_Start_Date,
                                  RE_End_Date = s.RE_End_Date,
                                  MgrCD = s.MgrCD,
                                  RE_Resign_Date = s.resignDate,
                                  RE_Sub_Rule_ID = s.subRuleID,
                                  RIC_SubmissionRule = s.subRule,
                                  RE_AnnualFeedBackFormID = s.RE_AnnualFeedBackFormID,
                                  RMS_Department=s.Department
                              }).ToList();

            return listOfEmployee;
        }
        //get the latest reporting history
        public IEnumerable<RIC_Employee> getAllUsersListWithLatestReporting()
        {
            IEnumerable<RIC_Employee> listOfEmployee = new List<RIC_Employee>();
            DateTime dtToday = SystemClock.US_Date.Date;
            var ReportingHistory = context.RIC_ReportingHistory;
            //get the latest reporting history Id.
            var LatestReportighistory = ReportingHistory.GroupBy(g => g.RR_EmpCD).Select(s => s.Max(m => m.RR_ID));

            // get the employees and manager from database.
            listOfEmployee = (from employee in
                                  (from employee in context.RIC_Employee
                                   join reportingHistory in ReportingHistory.Where(w=>LatestReportighistory.Contains(w.RR_ID))
                                            //  Where(s => (s.RR_ToDate == null ? dtToday : s.RR_ToDate) >= dtToday && s.RR_FromDate <= dtToday)
                                   on employee.RE_Emp_Cd equals reportingHistory.RR_EmpCD
                                   into rightJoin
                                   from reportingHistory in rightJoin.DefaultIfEmpty() // right join                                
                                   select new
                                   {
                                       empID = employee.RE_EmpId,
                                       EmpCd = employee.RE_Emp_Cd,
                                       EmpName = employee.RE_Employee_Name,
                                       RE_Jobdiva_User_Name = employee.RE_Jobdiva_User_Name,
                                       MgrCD = reportingHistory.RR_MgrCD,
                                       FromDate = reportingHistory.RR_FromDate,
                                       ToDate = reportingHistory.RR_ToDate,
                                       userRole = employee.RIC_User_Role,
                                       resignDate = employee.RE_Resign_Date,
                                       subRuleID = employee.RE_Sub_Rule_ID,
                                       SubmissionRule = employee.RIC_SubmissionRule,
                                       AnnualFeedBackFormID = employee.RE_AnnualFeedBackFormID
                                   })
                              join mgrTable in context.RIC_Employee
                             on employee.MgrCD equals mgrTable.RE_Emp_Cd
                             into leftjoin
                              from mgrTable in leftjoin.DefaultIfEmpty() // left join    
                              select new
                              {
                                  RE_EmpId = employee.empID,
                                  RE_Emp_Cd = employee.EmpCd,
                                  RE_Employee_Name = employee.EmpName,
                                  RE_Jobdiva_User_Name = employee.RE_Jobdiva_User_Name,
                                  ReportingTo = employee.MgrCD == "Directors" ? "Directors" : mgrTable.RE_Jobdiva_User_Name,
                                  RIC_User_Role = employee.userRole,
                                  RE_Start_Date = employee.FromDate,
                                  RE_End_Date = employee.ToDate,
                                  MgrCD = employee.MgrCD,
                                  resignDate = employee.resignDate,
                                  subRuleID = employee.subRuleID,
                                  subRule = employee.SubmissionRule,
                                  RE_AnnualFeedBackFormID = employee.AnnualFeedBackFormID
                              }).ToList().Select(s => new RIC_Employee()
                              {

                                  RE_EmpId = s.RE_EmpId,
                                  RE_Emp_Cd = s.RE_Emp_Cd,
                                  RE_Employee_Name = s.RE_Employee_Name,
                                  RE_Jobdiva_User_Name = s.RE_Jobdiva_User_Name,
                                  ReportingTo = s.ReportingTo,
                                  RIC_User_Role = s.RIC_User_Role,
                                  RE_Start_Date = s.RE_Start_Date,
                                  RE_End_Date = s.RE_End_Date,
                                  MgrCD = s.MgrCD,
                                  RE_Resign_Date = s.resignDate,
                                  RE_Sub_Rule_ID = s.subRuleID,
                                  RIC_SubmissionRule = s.subRule,
                                  RE_AnnualFeedBackFormID = s.RE_AnnualFeedBackFormID
                              }).ToList();

            return listOfEmployee;
        }
        public IEnumerable<ReportingHistoryResult> getDirectReportingHistory(string empCd)
        {
            // get the reporting history for the employee.
            IEnumerable<ReportingHistoryResult> reportingHistory = new List<ReportingHistoryResult>();
            reportingHistory = (from emp in context.RIC_Employee
                                join rh in context.RIC_ReportingHistory.Where(e => e.RR_EmpCD == empCd) on emp.RE_Emp_Cd equals rh.RR_EmpCD
                                join mgr in context.RIC_Employee    on rh.RR_MgrCD equals mgr.RE_Emp_Cd 
                                into repoartingTo from mgr in repoartingTo.DefaultIfEmpty()
                                select new ReportingHistoryResult
                                {
                                    RR_ID = rh.RR_ID,
                                    Employee_Name = emp.RE_Jobdiva_User_Name,
                                    Mgr_Name =rh.RR_MgrCD=="Directors"?"Directors": mgr.RE_Jobdiva_User_Name,
                                    RR_FromDate = rh.RR_FromDate,
                                    RR_ToDate = rh.RR_ToDate
                                }).ToList();
            return reportingHistory;
        }

        public IEnumerable<ReportingHistoryResult> getReportingHistory(string empCd, DateTime startDate,DateTime endDate, string role = null,string Department=null,int EmpID=0)
        {
            bool getAllrecord = false;  
            if (role == directorRoleName || role == AccMgrRoleName||role==HrRoleName||role==AdminRoleName)
                getAllrecord = true;
        
            Department = Department == null ? "All" : Department;
            if (role == AccMgrRoleName)
            {
                string accMgr = context.RIC_Employee.Where(s => s.RE_EmpId == EmpID).Select(s => s.RE_Employee_Name).FirstOrDefault().ToString();
                getAllrecord = false; Department = "REC";
                  var reportingHistory = context.Database.SqlQuery<ReportingHistoryResult>(
                                   "[SP_GetTreeview_AM]@FromDate,@ToDate,@EmpCd,@GetAllRecord,@Department,@EmpID",
                                    new SqlParameter("FromDate", startDate),
                                    new SqlParameter("ToDate", endDate),
                                    new SqlParameter("EmpCd", empCd),
                                    new SqlParameter("GetAllRecord", getAllrecord),
                                    new SqlParameter("Department", Department), 
                                    new SqlParameter("EmpID", EmpID)
                                    ).ToList();
                var ReportingHistory =
                (from emp in context.RIC_Employee.ToList()
                 join rhFunction in reportingHistory on emp.RE_Emp_Cd equals rhFunction.RR_EmpCD
                 join mgr in context.RIC_Employee.ToList() on rhFunction.RR_MgrCD equals mgr.RE_Emp_Cd
                    into mgrLeftJoin
                 from mgr in mgrLeftJoin.DefaultIfEmpty()
                 select new ReportingHistoryResult
                 {
                     RR_ID = rhFunction.RR_ID,
                     Employee_Name = emp.RE_Jobdiva_User_Name,
                     Mgr_Name = mgr != null ? mgr.RE_Jobdiva_User_Name : accMgr,
                     RR_FromDate = rhFunction.RR_FromDate,
                     RR_ToDate = rhFunction.RR_ToDate,
                     RR_EmpCD = rhFunction.RR_EmpCD,
                     RR_MgrCD = rhFunction.RR_MgrCD,
                     EmpLevel = rhFunction.EmpLevel
                 }).OrderBy(o => o.Employee_Name).ToList();
                return ReportingHistory;
            }
            else
            {
              var reportingHistory= context.Database.SqlQuery<ReportingHistoryResult>(
                                   "select *  FROM GetReportingHistory(@FromDate,@ToDate,@EmpCd,@GetAllRecord,@Department)",
                                    new SqlParameter("FromDate", startDate),
                                    new SqlParameter("ToDate", endDate),
                                    new SqlParameter("EmpCd", empCd),
                                    new SqlParameter("GetAllRecord", getAllrecord),
                                    new SqlParameter("Department", Department)
                                    ).ToList();
                var ReportingHistory =
                (from emp in context.RIC_Employee.ToList()
                 join rhFunction in reportingHistory on emp.RE_Emp_Cd equals rhFunction.RR_EmpCD
                 join mgr in context.RIC_Employee.ToList() on rhFunction.RR_MgrCD equals mgr.RE_Emp_Cd
                    into mgrLeftJoin
                 from mgr in mgrLeftJoin.DefaultIfEmpty()
                 select new ReportingHistoryResult
                 {
                     RR_ID = rhFunction.RR_ID,
                     Employee_Name = emp.RE_Jobdiva_User_Name,
                     Mgr_Name = mgr != null ? mgr.RE_Jobdiva_User_Name : "Directors",
                     RR_FromDate = rhFunction.RR_FromDate,
                     RR_ToDate = rhFunction.RR_ToDate,
                     RR_EmpCD = rhFunction.RR_EmpCD,
                     RR_MgrCD = rhFunction.RR_MgrCD,
                     EmpLevel = rhFunction.EmpLevel
                 }).OrderBy(o => o.Employee_Name).ToList();
                return ReportingHistory;
            }
            //var reportingHistory = context.Database.SqlQuery<ReportingHistoryResult>(
            //                        "select *  FROM GetReportingHistory(@FromDate,@ToDate,@EmpCd,@GetAllRecord,@Department)",
            //                         new SqlParameter("FromDate", startDate),
            //                         new SqlParameter("ToDate", endDate),
            //                         new SqlParameter("EmpCd", empCd),
            //                         new SqlParameter("GetAllRecord", getAllrecord),
            //                         new SqlParameter("Department",Department)
            //                         ).ToList();
            //var ReportingHistory = 
            //    (from emp in context.RIC_Employee.ToList()
            //     join rhFunction in reportingHistory on emp.RE_Emp_Cd equals rhFunction.RR_EmpCD
            //     join mgr in context.RIC_Employee.ToList() on rhFunction.RR_MgrCD equals mgr.RE_Emp_Cd
            //        into mgrLeftJoin from mgr in mgrLeftJoin.DefaultIfEmpty()
            //     select new ReportingHistoryResult
            //                    {
            //                RR_ID = rhFunction.RR_ID,
            //                Employee_Name = emp.RE_Jobdiva_User_Name,
            //                Mgr_Name = mgr != null ? mgr.RE_Jobdiva_User_Name : "Directors",
            //                RR_FromDate = rhFunction.RR_FromDate,
            //                RR_ToDate = rhFunction.RR_ToDate,
            //                RR_EmpCD=rhFunction.RR_EmpCD,
            //                RR_MgrCD=rhFunction.RR_MgrCD,
            //                EmpLevel=rhFunction.EmpLevel
            //                    }).OrderBy(o=>o.Employee_Name).ToList();
            //return ReportingHistory;
        }
    
        public IEnumerable<SelectListItem> getDepartmentList()
        {
            return context.RMS_Department.Select(s => new SelectListItem
            {
                Text = s.RD_Department,
                Value = s.RD_ID.ToString()
            });
        }

        public IEnumerable<SelectListItem> getClientList()
        {
            return context.RIC_Client.Select(s => new SelectListItem
            {
                Text = s.RC_ClientName,
                Value = s.RC_Id.ToString()
            });
        }

        public List<GetTargetItem> Get_TargetSubIntHire(string empCd, string role = null)
        {
            bool getAllrecord = false;

            if (role == directorRoleName)
                getAllrecord = true;

            var GetTarget = context.Database.SqlQuery<GetTargetItem>(
                                 "[SP_GetTargetSubIntHire] @EmpCd,@GetAllRecord",
                                  new SqlParameter("EmpCd", empCd),
                                  new SqlParameter("GetAllRecord", getAllrecord)
                                  ).ToList();
            return GetTarget;
        }

        public void InsertTarget(List<RIC_Targets> targetList)
        {
            context.RIC_Targets.AddRange(targetList);
        }
        public void InsertTargetHistory(List<RIC_TargetsHistory> targetList)
        {
            context.RIC_TargetsHistory.AddRange(targetList);
        }

        public void InsertTarget(RIC_Targets target)
        {
            context.RIC_Targets.Add(target);
        }

        public IEnumerable<RIC_Targets> GetTargetForTeam(string empCd,string month,int year )
        {
            return context.RIC_Targets
                   .Where(s => (s.RT_MgrCd == empCd || s.RT_Emp_Cd == empCd) && s.RT_Month == month && s.RT_Year == year)
                   .GroupBy(g => new{g.RT_Emp_Cd, g.RT_MgrCd})
                   .Select(s=>s.OrderByDescending(o=>o.RT_CreatedDate).FirstOrDefault())
                   .ToList();
        }


        //get monthly target for employee

        public void InsertMonthlyTarget(RIC_MonthlyTarget target)
        {
            //try
            //{
                context.RIC_MonthlyTarget.Add(target);

           // }
            //catch (Exception e)
            //{

                //throw;
            //}

        }
        public IEnumerable<RIC_MonthlyTarget> getemployeemonthlytarget(string empCd, string month,int year)
        {
            var monthlyTargets = context.Database.SqlQuery<RIC_MonthlyTarget>(
                                    "[SP_MonthlyTargetDetails] @EmpCd,@Month,@Year",
                                    new SqlParameter("EmpCd", empCd),
                                     new SqlParameter("Month", month),
                                     new SqlParameter("Year", year)

                                     ).ToList();
            IEnumerable<RIC_MonthlyTarget> monthlytargets = new List<RIC_MonthlyTarget>();
         


            var Monthlytargets = (from emp in monthlyTargets.
                                Where(e => (e.Emp_Cd == empCd)
                                      && (e.Month == month) && (e.Year == year))

                                  select new RIC_MonthlyTarget
                                  {
                                      Emp_Cd = emp.Emp_Cd,
                                      Emp_Name = emp.Emp_Name,
                                      SubmissionTarget = emp.SubmissionTarget,
                                      InterviewTarget = emp.InterviewTarget,
                                      HireTarget = emp.HireTarget,
                                      Margin = emp.Margin,
                                      Month = emp.Month,
                                      Year = emp.Year,
                                      Status = emp.Status,
                                      Team_Comments = emp.Team_Comments,
                                      Supervisor_Comments = emp.Supervisor_Comments


                                  }).ToList();
            foreach (var item in Monthlytargets)
            {
                item.Role = context.Database.SqlQuery<string>(
                       "[SP_GetEmployeeRoleName] @EmpCd",
                       new SqlParameter("EmpCd", item.Emp_Cd)
                        ).FirstOrDefault();
            }
            return Monthlytargets;
            //return monthlyTargets;
        }

        public IEnumerable<SP_EmployeeYearlyTarget> getemployeeYearlytarget(string empCd, int year,string SelectAll)
        {

            var monthlyTargets = context.Database.SqlQuery<SP_EmployeeYearlyTarget>(
                                    "[SP_MonthlyTargetDetails_Yearwise] @EmpCd,@SelectAll,@Year",
                                    new SqlParameter("EmpCd", empCd),
                                      new SqlParameter("SelectAll", SelectAll ),
                                     new SqlParameter("Year", year)

                                     ).ToList();
            IEnumerable<SP_EmployeeYearlyTarget> monthlytargets = new List<SP_EmployeeYearlyTarget>();
            var Monthlytargets = (from emp in monthlyTargets.
                                Where(e =>(e.Year == year))// (e.Emp_Cd == empCd)
                                
                                  select new SP_EmployeeYearlyTarget
                                  {
                                      Emp_Cd = emp.Emp_Cd,
                                      Emp_Name = emp.Emp_Name,
                                      SubmissionTarget = emp.SubmissionTarget,
                                      InterviewTarget = emp.InterviewTarget,
                                      HireTarget = emp.HireTarget,
                                      Margin = emp.Margin,
                                      Month = emp.Month,
                                      Year = emp.Year,
                                      Status = emp.Status,
                                      Team_Comments = emp.Team_Comments,
                                      Supervisor_Comments = emp.Supervisor_Comments,
                                      Manager_name=emp.Manager_name,
                                      Mgr_Cd=emp.Mgr_Cd


                                  }).ToList();
            foreach (var item in Monthlytargets)
            {
                item.Role = context.Database.SqlQuery<string>(
                       "[SP_GetEmployeeRoleName] @EmpCd",
                       new SqlParameter("EmpCd", item.Emp_Cd)
                        ).FirstOrDefault();
            }
            return Monthlytargets;
            //return monthlyTargets;
        }
        public IEnumerable< SP_MonthlyEmployeeResults> GetMonthlyData(string empCd, string MonthStartDate)
        {


            var GetTarget = context.Database.SqlQuery<SP_MonthlyEmployeeResults>(
                               "[SP_EmployeesMonthlyData] @EmpCd,@MonthStartDate1",
                                new SqlParameter("EmpCd", empCd),
                                new SqlParameter("MonthStartDate1", MonthStartDate)
                                ).ToList();
            return GetTarget;

        }
        public IEnumerable< SP_Last_Week_Dates> getPreviousWeekDate()
        {
            var GetTarget = context.Database.SqlQuery<SP_Last_Week_Dates>("[Sp_Last_weeks_Date]") ;
            return GetTarget;
        }

        public RIC_MonthlyTarget GetMonthlyTarget(string empcd,string month,int year)
        {
            var target = context.RIC_MonthlyTarget.Where(x=>(x.Emp_Cd== empcd) 
            && (x.Month == month)&&(x.Year == year)).SingleOrDefault();

            //target.Emp_Name = "test5";
            //context.SaveChanges();
            return target;

        }

        //Get Manager Code
        //public string GetManagerId(string empcd)
        public string GetManagerId(string empcd)
        {
            //var managercode = context.RIC_ReportingHistory.Where(x => (x.RR_EmpCD== empcd)).SingleOrDefault().RR_MgrCD;
            //var managercode = context.RIC_ReportingHistory.Where(x => (x.RR_EmpCD == empcd)).FirstOrDefault().RR_MgrCD;
            var managercodelst = context.RIC_ReportingHistory.
                Where(x => (x.RR_EmpCD == empcd) && x.RR_ToDate== null).
                Select(s => new { s.RR_MgrCD, s.RR_FromDate}).OrderByDescending(s=>s.RR_FromDate).ToList();
            var managercode = managercodelst.FirstOrDefault().RR_MgrCD;
            //target.Emp_Name = "test5";
            //context.SaveChanges();
            return managercode;
            //return string.Empty;

        }

        public DateTime GetToDate(string empcd)
        {
            var managercodelst = context.RIC_ReportingHistory.
                Where(x => (x.RR_EmpCD == empcd)).
                Select(s => new {s.RR_FromDate }).OrderByDescending(s => s.RR_FromDate).ToList();
            var todate = managercodelst.FirstOrDefault().RR_FromDate;
            //target.Emp_Name = "test5";
            //context.SaveChanges();
            return (DateTime)todate;
            //return string.Empty;

        }

        //Get Employee Name
        public string GetEmployeeName(string empcd)
        {
            var employeename = context.RIC_Employee.Where(x => (x.RE_Emp_Cd == empcd)).SingleOrDefault().RE_Employee_Name;
                        
            return employeename;

        }
        
        public List<RIC_MonthlyTarget> GetAllMgrEmployees(string empcd, string month, int year)
        {
            
            var lstemployeesmonthlytarget = context.RIC_MonthlyTarget.Where(x => (x.Mgr_Cd == empcd) 
            && (x.Month == month) && (x.Year == year)).ToList();
            //for getting current role
            foreach (var item in lstemployeesmonthlytarget)
            {
                            item.Role = context.Database.SqlQuery<string>(
                                   "[SP_GetEmployeeRoleName] @EmpCd",
                                   new SqlParameter("EmpCd", item.Emp_Cd)
                                    ).FirstOrDefault();
            }
                var todate = GetToDate(empcd);
            var lstmonthlytargetemployeecd = lstemployeesmonthlytarget.Select(x=>x.Emp_Cd).Distinct().ToList();
            //var lstemployeeCds = context.RIC_ReportingHistory.Where(x => x.RR_MgrCD == empcd). Select(x => x.RR_EmpCD).ToList();
            var lstemployeeCds = context.RIC_ReportingHistory.Where(x => (x.RR_MgrCD == empcd) 
            && (x.RR_ToDate == null)).
                Select(x => x.RR_EmpCD).ToList();

            var lstemployeeCdsnew = lstemployeeCds.Except(lstmonthlytargetemployeecd);
            var lstemployees = context.RIC_Employee.Where(x=> lstemployeeCdsnew.Contains(x.RE_Emp_Cd)).Select(x=>new 
             {EmpCd = x.RE_Emp_Cd, EmpName = x.RE_Employee_Name }).ToList();

            foreach (var item in lstemployees)
            {
                RIC_MonthlyTarget obj = new RIC_MonthlyTarget();
                obj.Emp_Cd = item.EmpCd;
                obj.Emp_Name = item.EmpName;
                obj.SubmissionTarget = 0;
                obj.InterviewTarget = 0;
                obj.HireTarget = 0;
                obj.Team_Comments = string.Empty;
                obj.Supervisor_Comments = string.Empty;
                obj.Status = "Pending";
                obj.Month = month;
                obj.Year = year;
                obj.Role = context.Database.SqlQuery<string>(
                                   "[SP_GetEmployeeRoleName] @EmpCd",
                                   new SqlParameter("EmpCd", item.EmpCd)
                                    ).FirstOrDefault();
                lstemployeesmonthlytarget.Add(obj);
            }
            //target.Emp_Name = "test5";
            //context.SaveChanges();
            return lstemployeesmonthlytarget;

        }


        public List<RIC_Targets> getAllTargets(Expression<Func<RIC_Targets, bool>> filter = null,
                                            Func<IQueryable<RIC_Targets>, IOrderedQueryable<RIC_Targets>> orderBy = null
                                           )
        {
            // string includeProperties = ""
             IQueryable<RIC_Targets> query = context.RIC_Targets.AsQueryable();
            if (filter != null)
            {
                
                query= query.Where(filter);
                query = query.Where(filter);
            }
            //// include the properties.
            //foreach (var includeProperty in includeProperties.Split
            //    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    query = query.Include(includeProperty);
            //}

            if (orderBy != null)
            {
                return orderBy(query).ToList();//retun list by order.
            }
            else
            {
                return query.ToList();// retuurn list.  
            }
           // return context.RIC_Targets.ToList();
        }

        public List<RIC_TargetsHistory> getAllTargetsHistory(Expression<Func<RIC_TargetsHistory, bool>> filter = null,
                                            Func<IQueryable<RIC_TargetsHistory>, IOrderedQueryable<RIC_TargetsHistory>> orderBy = null
                                           )
        {
            // string includeProperties = ""
            IQueryable<RIC_TargetsHistory> query = context.RIC_TargetsHistory.AsQueryable();
            if (filter != null)
            {

                query = query.Where(filter);
                query = query.Where(filter);
            }
            //// include the properties.
            //foreach (var includeProperty in includeProperties.Split
            //    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    query = query.Include(includeProperty);
            //}

            if (orderBy != null)
            {
                return orderBy(query).ToList();//retun list by order.
            }
            else
            {
                return query.ToList();// retuurn list.  
            }
            // return context.RIC_Targets.ToList();
        }

        public List<RIC_UserDashboardPanel> getDashboardPanelsForUser(Expression<Func<RIC_UserDashboardPanel, bool>> filter = null,
                                            Func<IQueryable<RIC_UserDashboardPanel>, IOrderedQueryable<RIC_UserDashboardPanel>> orderBy = null
                                             , string includeProperties = "")
        {
            IQueryable<RIC_UserDashboardPanel> query = context.RIC_UserDashboardPanel.AsQueryable();
            if (filter != null)
            {
                query = query.Where(filter);
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
              (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                return orderBy(query).ToList();//retun list by order.
            }
            else
            {
                return query.ToList();// retuurn list.  
            }

        }

        public List<RIC_Panel> getPanelsList(Expression<Func<RIC_Panel, bool>> filter = null,
                                            Func<IQueryable<RIC_Panel>, IOrderedQueryable<RIC_Panel>> orderBy = null,
                                             string includeProperties = "")
        {
            IQueryable<RIC_Panel> query = context.RIC_Panel;
            if (filter != null)
            {

                query = query.Include("").Where(filter);
                query = query.Where(filter);
            }
            foreach (var includeProperty in includeProperties.Split
               (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                return orderBy(query).ToList();//retun list by order.
            }
            else
            {
                return query.ToList();// retuurn list.  
            }
        }
        public void UpdateDashboardPanel(List<RIC_UserDashboardPanel> panel ,string empCd)
        {
           
            List<RIC_UserDashboardPanel> pList = context.RIC_UserDashboardPanel
                       .Where(s => s.RU_EmpCd == empCd).ToList();

            List<RIC_UserDashboardPanel> itemToDelete = pList.Where(s => !panel.Select(p => p.RU_PanelId).Contains(s.RU_PanelId)).ToList();
            context.RIC_UserDashboardPanel.RemoveRange(itemToDelete);
            foreach (var item in panel)
            {
                var pitem = pList.SingleOrDefault(s => s.RU_PanelId == item.RU_PanelId);
                if (pitem == null)
                {
                    context.RIC_UserDashboardPanel.Add(item);
                }
                else
                {
                    pitem.RU_ColumnNumber = item.RU_ColumnNumber;
                    pitem.RU_SortOrder = item.RU_SortOrder;
                    pitem.RU_ColumnWidth = item.RU_ColumnWidth;
                }

            }
           // var empCd=panel.FirstOrDefault().RU_EmpCd;
           

          //  context.RIC_UserDashboardPanel.RemoveRange(pList);
           // context.RIC_UserDashboardPanel.AddRange(panel);
            //context.RIC_UserDashboardPanel.AddRange(panel);
        }

        public DataSet getAuditDetails()
        {
            //SqlParameter param; IDataReader reader1;
            DataSet ds = new DataSet();
            var command = context.Database.Connection.CreateCommand();
            context.Database.Connection.Open();
            //var con = context.Database.Connection;
            command.CommandText = "[dbo].[SP_getAuditdetails]";
            command.CommandType = CommandType.StoredProcedure;

            //var adapert = new SqlDataAdapter(command);
            var reader = command.ExecuteReader();

            while (!reader.IsClosed)
                ds.Tables.Add().Load(reader);
            return ds;

        }



    }
}
