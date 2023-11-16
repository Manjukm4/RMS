using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary.Repository
{
    public class SharedReqRepository<TEntity> : GenericRepositiory<TEntity> where TEntity : RMS_SharedReq_HDR
    {
        public SharedReqRepository(RIC_DBEntities context)
            : base(context) // call the base constructor
        {

        }

        public IEnumerable<String> GetCompany(string Prefix)
        {
            IEnumerable<string> CompanyName =
                  context.Database.SqlQuery<string>(
                                "[dbo].[SP_GetCompany] @Prefix",
                                 new SqlParameter("@Prefix", Prefix));

            return CompanyName;
        }

    }
}
