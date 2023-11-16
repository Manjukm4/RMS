namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Nullabledattime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_SharedReqSubmittals", "RS_EntredDate", c => c.DateTime());
            AlterColumn("dbo.RMS_SharedReqSubmittals", "RS_SubmittedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReqSubmittals", "RS_SubmittedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RMS_SharedReqSubmittals", "RS_EntredDate", c => c.DateTime(nullable: false));
        }
    }
}
