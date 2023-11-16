namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncentivesYear : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Incentive", "Year", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Incentive", "Year");
        }
    }
}
