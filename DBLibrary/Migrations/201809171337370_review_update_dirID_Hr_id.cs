namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class review_update_dirID_Hr_id : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Review", "RR_DirCd", c => c.String(maxLength: 50));
            AddColumn("dbo.RIC_Review", "RR_HrCd", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RIC_Review", "RR_HrCd");
            DropColumn("dbo.RIC_Review", "RR_DirCd");
        }
    }
}
