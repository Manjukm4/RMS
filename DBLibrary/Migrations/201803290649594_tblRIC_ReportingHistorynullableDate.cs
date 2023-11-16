namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tblRIC_ReportingHistorynullableDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_ReportingHistory", "RR_FromDate", c => c.DateTime());
            AlterColumn("dbo.RIC_ReportingHistory", "RR_ToDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_ReportingHistory", "RR_ToDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RIC_ReportingHistory", "RR_FromDate", c => c.DateTime(nullable: false));
        }
    }
}
