namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IncentivesYearNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Incentive", "Year", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Incentive", "Year", c => c.Int(nullable: false));
        }
    }
}
