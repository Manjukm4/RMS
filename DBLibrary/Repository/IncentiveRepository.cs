using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBLibrary.Repository
{
    public class IncentiveRepository<TEntity> : GenericRepositiory<TEntity> where TEntity : RIC_Incentive
    {
        public IncentiveRepository(RIC_DBEntities context)
            : base(context) // call the base constructor
        {
        }

        public List<GetIncentiveResult> getTncetive(string empCd,int Year)
        {
                    
            var result=
             context.Database.SqlQuery<GetIncentiveResult>("[dbo].[Sp_GetIncentives] @EmpCd,@Year", new SqlParameter("EmpCd", empCd),
                                                                                                new SqlParameter("Year",Year)).ToList();
            return result;
        }

        public IEnumerable<Get_All_IncentivesResult> getAllIncentives()
        {
            var result =
             context.Database.SqlQuery<Get_All_IncentivesResult>("[dbo].[SP_UnpivotIncentives]").ToList();
            return result;

        }




    }      

}


