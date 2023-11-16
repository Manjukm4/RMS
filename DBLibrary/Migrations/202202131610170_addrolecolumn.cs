namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addrolecolumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_MonthlyTarget", "Role", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_MonthlyTarget", "Role");
        }
    }
}
