namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Submission_Tbl_09_03_2018 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Submission",
                c => new
                    {
                        RS_Id = c.Int(nullable: false, identity: true),
                        RS_EmpId = c.String(maxLength: 50),
                        RS_RecruterName = c.String(maxLength: 50),
                        RS_FromDt = c.DateTime(nullable: false),
                        RS_ToDt = c.DateTime(nullable: false),
                        RS_Email = c.String(maxLength: 500),
                        RS_Division = c.String(maxLength: 50),
                        RS_Submission = c.Int(nullable: false),
                        RS_Interviews = c.Int(nullable: false),
                        RS_Hires = c.Int(nullable: false),
                        RS_NetbillHires_Hour = c.Single(nullable: false),
                        RS_CostOfHires_Hour = c.Single(nullable: false),
                        RS_MarginOfHires_Hour = c.Single(nullable: false),
                        RS_MarginOfHires_Per = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.RS_Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_Submission");
        }
    }
}
