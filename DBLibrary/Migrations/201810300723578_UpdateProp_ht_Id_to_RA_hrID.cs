namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateProp_ht_Id_to_RA_hrID : DbMigration
    {
        public override void Up()
        {
            RenameColumn("dbo.RIC_AnnualFeedback", "Hr_Id", "RA_HrId");
           // AddColumn("dbo.RIC_AnnualFeedback", "RA_HrId", c => c.String(maxLength: 50));
           // DropColumn("dbo.RIC_AnnualFeedback", "Hr_Id");
        }
        
        public override void Down()
        {
            RenameColumn("dbo.RIC_AnnualFeedback", "Hr_Id", "RA_HrId");
           // AddColumn("dbo.RIC_AnnualFeedback", "Hr_Id", c => c.String(maxLength: 50));
            //DropColumn("dbo.RIC_AnnualFeedback", "RA_HrId");
        }
    }
}
