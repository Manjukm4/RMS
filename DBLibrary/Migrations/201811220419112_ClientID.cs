namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ClientID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_Dtl", "RS_ClientID", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_SharedReq_Dtl", "RS_ClientID");
        }
    }
}
