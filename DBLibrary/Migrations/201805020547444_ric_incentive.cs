namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ric_incentive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Incentive", "RI_Sep_Incentive", c => c.Single());
            DropColumn("dbo.RIC_Incentive", "RI_Sup_Incentive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RIC_Incentive", "RI_Sup_Incentive", c => c.Single());
            DropColumn("dbo.RIC_Incentive", "RI_Sep_Incentive");
        }
    }
}
