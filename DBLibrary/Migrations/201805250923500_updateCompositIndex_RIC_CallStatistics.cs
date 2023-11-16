namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCompositIndex_RIC_CallStatistics : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.RIC_Call_Statistics", "IX_RC_Composite");
            CreateIndex("dbo.RIC_Call_Statistics", new[] { "RC_Emp_Cd", "RC_Date", "RC_Time", "RC_CallType", "RC_Dailed", "RC_Calling" }, unique: true, name: "IX_RC_Composite");
        }
        
        public override void Down()
        {
            DropIndex("dbo.RIC_Call_Statistics", "IX_RC_Composite");
            CreateIndex("dbo.RIC_Call_Statistics", new[] { "RC_Emp_Cd", "RC_Date", "RC_Time", "RC_CallType" }, unique: true, name: "IX_RC_Composite");
        }
    }
}
