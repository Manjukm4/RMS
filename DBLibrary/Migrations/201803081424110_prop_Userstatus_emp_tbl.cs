namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class prop_Userstatus_emp_tbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RE_User_Status", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "RE_User_Status");
        }
    }
}
