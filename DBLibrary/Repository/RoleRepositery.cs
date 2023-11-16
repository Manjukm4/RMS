using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DBLibrary.Repository
{
    public class RoleRepositery<TEntity> : GenericRepositiory<TEntity> where TEntity : RIC_Role   
    {
        public RoleRepositery(RIC_DBEntities context)
            : base(context) { }// call the base constructor

        public List<SelectListItem> getRoleList()
        {
            // bind the manager list.
            List<SelectListItem> _lstSelectedItem = new List<SelectListItem>();
            var roleList = dbSet.Where(s=>s.RR_RoleId!=1);    //entities.usp_GetRoles();
            foreach (var role in roleList)// add the items in select list.
            {
                SelectListItem selectListItem = new SelectListItem();
                selectListItem.Text = role.RR_Role_Name;
                selectListItem.Value = role.RR_RoleId.ToString();
                _lstSelectedItem.Add(selectListItem);
            }
            //user.ManagerList = _mgrSelectedItem;
            return _lstSelectedItem;
        }

    }
}
