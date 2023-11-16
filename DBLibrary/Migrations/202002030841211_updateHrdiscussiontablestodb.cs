namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateHrdiscussiontablestodb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_CreatedDate", c => c.DateTime());
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_UpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_UpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
