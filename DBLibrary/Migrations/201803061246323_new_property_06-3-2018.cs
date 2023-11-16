namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class new_property_0632018 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RE_Start_Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.RIC_Employee", "RE_End_Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "RE_End_Date");
            DropColumn("dbo.RIC_Employee", "RE_Start_Date");
        }
    }
}
