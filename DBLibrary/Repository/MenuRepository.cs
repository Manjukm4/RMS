using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary.Repository
{
   public class MenuRepository<TEntity> : GenericRepositiory<TEntity> where TEntity : RIC_Menu_Module
    {
        public MenuRepository(RIC_DBEntities context)
          : base(context) // call the base constructor
        {
        }



    }
}
