namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Filetypelength : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_FileType", c => c.String(maxLength: 100));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_FileType", c => c.String(maxLength: 50));
        }
    }
}
