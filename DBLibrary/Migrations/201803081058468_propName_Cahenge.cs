namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class propName_Cahenge : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RE_AKA_Name", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.RIC_Employee", "AKA_Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_Employee", "AKA_Name", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.RIC_Employee", "RE_AKA_Name");
        }
    }
}
