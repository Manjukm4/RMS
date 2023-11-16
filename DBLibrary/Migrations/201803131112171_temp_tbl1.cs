namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class temp_tbl1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_SubmissionsTemp", "EndDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_SubmissionsTemp", "EndDate", c => c.DateTime(nullable: false));
        }
    }
}
