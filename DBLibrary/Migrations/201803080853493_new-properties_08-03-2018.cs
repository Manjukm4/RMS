namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newproperties_08032018 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RE_Exp", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "RE_Exp");
        }
    }
}
