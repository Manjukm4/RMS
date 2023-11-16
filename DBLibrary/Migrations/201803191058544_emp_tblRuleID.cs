namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class emp_tblRuleID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Employee", "RE_Sub_Rule_ID", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Employee", "RE_Sub_Rule_ID");
        }
    }
}
