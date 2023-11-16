namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SharedUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_Dtl", "RS_Priority", c => c.String(maxLength: 50));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_WorkLocation", c => c.String(maxLength: 10));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_Division", c => c.String(maxLength: 50));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_Category", c => c.String(maxLength: 50));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_State", c => c.String(maxLength: 10));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_PrimaryAssigment", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_PrimaryAssigment", c => c.String(maxLength: 100));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_State", c => c.String(maxLength: 100));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_Category", c => c.String(maxLength: 100));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_Division", c => c.String(maxLength: 100));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_WorkLocation", c => c.String(maxLength: 100));
            DropColumn("dbo.RMS_SharedReq_Dtl", "RS_Priority");
        }
    }
}
