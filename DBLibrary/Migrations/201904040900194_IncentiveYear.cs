namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncentiveYear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Incentive", "RI_Year", c => c.Int());
            DropColumn("dbo.RIC_Incentive", "Year");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_Incentive", "Year", c => c.Int());
            DropColumn("dbo.RIC_Incentive", "RI_Year");
        }
    }
}
