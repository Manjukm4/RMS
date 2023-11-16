namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class designation9 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Employee", "RE_DesignationID", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Employee", "RE_DesignationID", c => c.Int(nullable: false));
        }
    }
}
