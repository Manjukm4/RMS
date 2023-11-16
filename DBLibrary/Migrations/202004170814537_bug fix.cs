namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bugfix : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.RMS_JobChecked_Info", name: "RC_RefHDRID", newName: "RJ_RefHDRID");
            RenameIndex(table: "dbo.RMS_JobChecked_Info", name: "IX_RC_RefHDRID", newName: "IX_RJ_RefHDRID");
            DropPrimaryKey("dbo.RMS_JobChecked_Info");
            
            DropColumn("dbo.RMS_JobChecked_Info", "RC_ID");
            AddColumn("dbo.RMS_JobChecked_Info", "RJ_ID", c => c.Int(nullable: false, identity: true));
            AddColumn("dbo.RMS_JobChecked_Info", "RJ_CheckedTo", c => c.String(maxLength: 10));
            AddColumn("dbo.RMS_JobChecked_Info", "RJ_CheckedInDt", c => c.DateTime(nullable: false));
            AddColumn("dbo.RMS_JobChecked_Info", "RJ_CheckedOutDt", c => c.DateTime(nullable: false));
            AddPrimaryKey("dbo.RMS_JobChecked_Info", "RJ_ID");
            DropColumn("dbo.RMS_JobChecked_Info", "CheckedTo");
            DropColumn("dbo.RMS_JobChecked_Info", "RS_CheckedInDt");
            DropColumn("dbo.RMS_JobChecked_Info", "RS_CheckedOutDt");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RMS_JobChecked_Info", "RS_CheckedOutDt", c => c.DateTime(nullable: false));
            AddColumn("dbo.RMS_JobChecked_Info", "RS_CheckedInDt", c => c.DateTime(nullable: false));
            AddColumn("dbo.RMS_JobChecked_Info", "CheckedTo", c => c.String(maxLength: 10));
            AddColumn("dbo.RMS_JobChecked_Info", "RC_ID", c => c.Int(nullable: false, identity: true));
            DropPrimaryKey("dbo.RMS_JobChecked_Info");
            DropColumn("dbo.RMS_JobChecked_Info", "RJ_CheckedOutDt");
            DropColumn("dbo.RMS_JobChecked_Info", "RJ_CheckedInDt");
            DropColumn("dbo.RMS_JobChecked_Info", "RJ_CheckedTo");
            DropColumn("dbo.RMS_JobChecked_Info", "RJ_ID");
            AddPrimaryKey("dbo.RMS_JobChecked_Info", "RC_ID");
            RenameIndex(table: "dbo.RMS_JobChecked_Info", name: "IX_RJ_RefHDRID", newName: "IX_RC_RefHDRID");
            RenameColumn(table: "dbo.RMS_JobChecked_Info", name: "RJ_RefHDRID", newName: "RC_RefHDRID");
        }
    }
}
