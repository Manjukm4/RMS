namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatehrdiscussiontablestodb : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_ReviewDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_HRDiscussion_Hdr", "RH_ReviewDate", c => c.DateTime(nullable: false));
        }
    }
}
