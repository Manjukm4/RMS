namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Margin : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_MonthlyTarget", "Margin", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.RIC_MonthlyTarget", "Account_Name", c => c.String());
            AddColumn("dbo.RIC_MonthlyTarget", "Starts", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_MonthlyTarget", "Starts");
            DropColumn("dbo.RIC_MonthlyTarget", "Account_Name");
            DropColumn("dbo.RIC_MonthlyTarget", "Margin");
        }
    }
}
