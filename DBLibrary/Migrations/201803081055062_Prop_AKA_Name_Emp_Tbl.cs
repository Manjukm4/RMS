namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Prop_AKA_Name_Emp_Tbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "AKA_Name", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "AKA_Name");
        }
    }
}
