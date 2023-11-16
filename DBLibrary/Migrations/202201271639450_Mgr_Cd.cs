namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Mgr_Cd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_MonthlyTarget", "Mgr_Cd", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_MonthlyTarget", "Mgr_Cd");
        }
    }
}
