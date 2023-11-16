namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RIC_Holidays_tbl : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RIC_Holidays",
                c => new
                    {
                        RH_Id = c.Int(nullable: false, identity: true),
                        RH_Date = c.DateTime(nullable: false),
                        RH_Festival = c.String(),
                    })
                .PrimaryKey(t => t.RH_Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RIC_Holidays");
        }
    }
}
