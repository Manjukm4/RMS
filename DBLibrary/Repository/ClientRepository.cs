using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace DBLibrary.Repository
{
    public class ClientRepository<TEntity> : GenericRepositiory<TEntity> where TEntity : RIC_Client
    {
        public ClientRepository(RIC_DBEntities context)
            : base(context) // call the base constructor
        {
        }

        //Insert data into database
        public void Insert(RIC_Client client)
        {
            context.RIC_Client.Add(client);
        }

        //Get data from database
        public List<RIC_Client> getClientList()
        {
            List<RIC_Client> getClient = new List<RIC_Client>();
            // get the ClientList fromn database.
            var list = (from client in context.RIC_Client

                        select new
                        {
                            RC_Id = client.RC_Id,
                            RC_ClientName = client.RC_ClientName,

                        }).ToList();
            // var listOfClients=                     
            foreach (var item in list) //retrieve each item and assign to model
            {
                getClient.Add(new RIC_Client
                {
                    RC_Id = item.RC_Id,
                    RC_ClientName = item.RC_ClientName,

                });
            }
            return getClient;




        }


        //Delete data in database
        public void DeleteRecord(int rc_id)
        {
            var RIC_Client_rReCord = (from c in context.RIC_Client
                                      where c.RC_Id == rc_id
                                      select c).FirstOrDefault();
            if (RIC_Client_rReCord != null)
            {
                context.RIC_Client.Remove(RIC_Client_rReCord);
                context.SaveChanges();
            }


        }

        //Update data in database
        public void UpdateRecord(string clientname, int RC_Id)
        {
            var RIC_Client_rReCord = (from c in context.RIC_Client
                                      where c.RC_Id == RC_Id
                                      select c).FirstOrDefault();
            if (RIC_Client_rReCord != null)
            {
                RIC_Client_rReCord.RC_ClientName = clientname;
                context.SaveChanges();
            }


        }
    }
}