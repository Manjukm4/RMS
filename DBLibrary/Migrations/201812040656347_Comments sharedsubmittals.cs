namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Commentssharedsubmittals : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReqSubmittals", "RS_Comments", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RMS_SharedReqSubmittals", "RS_Comments");
        }
    }
}
