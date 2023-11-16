namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class targetReportinghistoryForignKey : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Targets", "RT_ReportingId", c => c.Int(nullable: false));
            CreateIndex("dbo.RIC_Targets", "RT_ReportingId");
            AddForeignKey("dbo.RIC_Targets", "RT_ReportingId", "dbo.RIC_ReportingHistory", "RR_ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RIC_Targets", "RT_ReportingId", "dbo.RIC_ReportingHistory");
            DropIndex("dbo.RIC_Targets", new[] { "RT_ReportingId" });
            DropColumn("dbo.RIC_Targets", "RT_ReportingId");
        }
    }
}
