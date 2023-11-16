namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class password_Length : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Employee", "RE_Password", c => c.String(nullable: false, maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Employee", "RE_Password", c => c.String(nullable: false, maxLength: 5));
        }
    }
}
