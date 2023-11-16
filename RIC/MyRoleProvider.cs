using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using DBLibrary.UnitOfWork;
namespace RIC
{
    public class MyRoleProvider : RoleProvider
    {
        private UnitOfWork unitOfWork;
        public MyRoleProvider()
        {
            unitOfWork = new UnitOfWork();
        }


        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username)
        {
            // get the user from db.
            var objUser = unitOfWork.User.GetByEmpID(username);
            if (objUser == null)
            {
                return null;
            }
            else
            {
                // get the roles for the user.              
                string[] ret = objUser.RIC_User_Role.First().RIC_Role.RR_Role_Name.Split(';');
                return ret;
            }
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}