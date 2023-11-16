namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class employeeshortel_Name : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RE_Shortel_Name", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "RE_Shortel_Name");
        }
    }
}
