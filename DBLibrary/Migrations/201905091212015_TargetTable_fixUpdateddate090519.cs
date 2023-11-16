namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TargetTable_fixUpdateddate090519 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_Targets", "RT_UpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_Targets", "RT_UpdatedDate", c => c.DateTime(nullable: false));
        }
    }
}
