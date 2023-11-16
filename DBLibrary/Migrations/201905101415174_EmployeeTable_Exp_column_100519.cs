namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmployeeTable_Exp_column_100519 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RE_Experience", c => c.String(maxLength: 30));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "RE_Experience");
        }
    }
}
