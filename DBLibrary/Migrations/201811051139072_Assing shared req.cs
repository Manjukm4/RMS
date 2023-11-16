namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Assingsharedreq : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RMS_AssignSharedReq",
                c => new
                    {
                        RA_ID = c.Int(nullable: false, identity: true),
                        RA_SharedReqHDRID = c.Int(nullable: false),
                        RA_AssignedTo = c.String(maxLength: 50),
                        RA_Instructions = c.String(maxLength: 1000),
                        RA_CreatedBy = c.String(maxLength: 50),
                        CreatedDt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RA_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RMS_AssignSharedReq");
        }
    }
}
