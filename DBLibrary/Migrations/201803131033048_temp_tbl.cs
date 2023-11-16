namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temp_tbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_SubmissionsTemp",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        EmpCd = c.String(nullable: false),
                        EmpName = c.String(nullable: false),
                        StrtDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        CompanyName = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_SubmissionsTemp");
        }
    }
}
