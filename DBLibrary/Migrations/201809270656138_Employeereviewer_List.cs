namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Employeereviewer_List : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RE_ReviewerList", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "RE_ReviewerList");
        }
    }
}
