namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class table_RIC_ReportingHistoryNewProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_ReportingHistory", "RR_CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_ReportingHistory", "RR_UpdatedBy", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_ReportingHistory", "RR_UpdatedBy");
            DropColumn("dbo.RIC_ReportingHistory", "RR_CreatedDate");
        }
    }
}
