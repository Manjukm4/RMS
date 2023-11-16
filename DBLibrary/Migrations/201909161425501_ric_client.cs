namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ric_client : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Client", "RC_ClientName", c => c.String(maxLength: 255));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Client", "RC_ClientName", c => c.String(maxLength: 50));
        }
    }
}
