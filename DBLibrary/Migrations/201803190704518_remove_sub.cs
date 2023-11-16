namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class remove_sub : DbMigration
    {
        public override void Up()
        {
            DropTable("dbo.RIC_SubmissionDaily");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.RIC_SubmissionDaily",
                c => new
                    {
                        RS_Sub = c.Int(nullable: false, identity: true),
                        RS_Emp_Cd = c.String(nullable: false, maxLength: 50),
                        RS_Date = c.DateTime(nullable: false),
                        RS_Submissions = c.Int(nullable: false),
                        RS_Interviews = c.Int(nullable: false),
                        RS_Hires = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RS_Sub);
            
        }
    }
}
