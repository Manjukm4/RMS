namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcolumnsintoRIC_ClientMapping : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_ClientMapping", "RCM_TeamLeadId", c => c.String());
            AddColumn("dbo.RIC_ClientMapping", "RCM_StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_ClientMapping", "RCM_EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_ClientMapping", "RCM_EndDate");
            DropColumn("dbo.RIC_ClientMapping", "RCM_StartDate");
            DropColumn("dbo.RIC_ClientMapping", "RCM_TeamLeadId");
        }
    }
}
