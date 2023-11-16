namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BugFixforJobCheckedTbl : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_JobChecked_Info", "RJ_CheckedInDt", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_JobChecked_Info", "RJ_CheckedInDt", c => c.DateTime(nullable: false));
        }
    }
}
