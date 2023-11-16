namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRicIncentive : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Incentive", "RI_TotalRecurringPaid", c => c.Single());
            AddColumn("dbo.RIC_Incentive", "RI_OneTimeIncentive", c => c.Single());
            AddColumn("dbo.RIC_Incentive", "RI_finalDifference", c => c.Single());
            AddColumn("dbo.RIC_Incentive", "RI_Remarks", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Incentive", "RI_Remarks");
            DropColumn("dbo.RIC_Incentive", "RI_finalDifference");
            DropColumn("dbo.RIC_Incentive", "RI_OneTimeIncentive");
            DropColumn("dbo.RIC_Incentive", "RI_TotalRecurringPaid");
        }
    }
}
