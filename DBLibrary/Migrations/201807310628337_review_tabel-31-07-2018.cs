namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class review_tabel31072018 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Review", "RR_ReviewDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Review", "RR_ReviewDate");
        }
    }
}
