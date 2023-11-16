namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tbl_RIC_SubmissionDaily : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_SubmissionDaily",
                c => new
                    {
                        RS_Sub_ID = c.Int(nullable: false, identity: true),
                        RS_Emp_Cd = c.String(nullable: false, maxLength: 50),
                        RS_Date = c.DateTime(nullable: false),
                        RS_Submissions = c.Int(nullable: false),
                        RS_Interviews = c.Int(nullable: false),
                        RS_Hires = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RS_Sub_ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_SubmissionDaily");
        }
    }
}
