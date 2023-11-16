namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tblRIC_ReportingHistory : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_ReportingHistory",
                c => new
                    {
                        RR_ID = c.Int(nullable: false, identity: true),
                        RR_EmpCD = c.String(maxLength: 50),
                        RR_MgrCD = c.String(maxLength: 50),
                        RR_FromDate = c.DateTime(nullable: false),
                        RR_ToDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RR_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_ReportingHistory");
        }
    }
}
