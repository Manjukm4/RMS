namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatePropEmployee_23032018 : DbMigration
    {
        public override void Up()
        {
           // AddColumn("dbo.RIC_Employee", "RE_Employee_Name", c => c.String(nullable: false, maxLength: 50));
           // DropColumn("dbo.RIC_Employee", "RE_First_Name");
            DropColumn("dbo.RIC_Employee", "RE_Last_Name");

            RenameColumn("dbo.RIC_Employee", "RE_First_Name", "RE_Employee_Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_Employee", "RE_Last_Name", c => c.String(nullable: false, maxLength: 50));
           // AddColumn("dbo.RIC_Employee", "RE_First_Name", c => c.String(nullable: false, maxLength: 50));
           // DropColumn("dbo.RIC_Employee", "RE_Employee_Name");
            RenameColumn("dbo.MyTable", "RE_Employee_Name", "RE_First_Name");
            
        }
    }
}
