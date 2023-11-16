namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_uniqueKye_callStatistics : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RIC_Call_Statistics", "IX_RC_Composite");
            CreateIndex("dbo.RIC_Call_Statistics", new[] { "RC_Emp_Cd", "RC_Date", "RC_Time", "RC_CallType" }, unique: true, name: "IX_RC_Composite");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RIC_Call_Statistics", "IX_RC_Composite");
            CreateIndex("dbo.RIC_Call_Statistics", new[] { "RC_Emp_Cd", "RC_Date", "RC_Time" }, unique: true, name: "IX_RC_Composite");
        }
    }
}
