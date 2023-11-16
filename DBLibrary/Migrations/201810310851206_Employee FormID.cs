namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeFormID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RE_AnnualFeedBackFormID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "RE_AnnualFeedBackFormID");
        }
    }
}
