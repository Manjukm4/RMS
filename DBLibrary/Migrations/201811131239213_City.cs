namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class City : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RMS_SharedReq_Dtl", "RS_City", c => c.String(maxLength: 20));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_WorkLocation", c => c.String(maxLength: 500));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_State", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_State", c => c.String(maxLength: 10));
            AlterColumn("dbo.RMS_SharedReq_Dtl", "RS_WorkLocation", c => c.String(maxLength: 200));
            DropColumn("dbo.RMS_SharedReq_Dtl", "RS_City");
        }
    }
}
