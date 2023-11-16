namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tblCallStatistics_22032018 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Call_Statistics",
                c => new
                    {
                        RC_Id = c.Int(nullable: false, identity: true),
                        RC_Emp_Cd = c.String(nullable: false, maxLength: 50),
                        RC_Date = c.DateTime(nullable: false),
                        RC_Time = c.Time(nullable: false, precision: 7),
                        RC_CallType = c.String(nullable: false, maxLength: 5),
                        RC_Dailed = c.String(nullable: false, maxLength: 20),
                        RC_Calling = c.String(nullable: false, maxLength: 20),
                        RC_Duration = c.Time(nullable: false, precision: 7),
                        RC_Call_Connected = c.Int(nullable: false),
                        RC_Voice_Message = c.Int(nullable: false),
                        RC_PRI = c.String(maxLength: 20),
                    })
                .PrimaryKey(t => t.RC_Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_Call_Statistics");
        }
    }
}
