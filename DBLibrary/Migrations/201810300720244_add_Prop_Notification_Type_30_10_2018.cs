namespace DBLibrary.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Prop_Notification_Type_30_10_2018 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RIC_Notification", "RN_NotificationType", c => c.String(maxLength: 100));
            AlterColumn("dbo.RIC_AnnualFeedbackFields", "AF_FieldName", c => c.String(maxLength: 200));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RIC_AnnualFeedbackFields", "AF_FieldName", c => c.String(maxLength: 100));
            DropColumn("dbo.RIC_Notification", "RN_NotificationType");
        }
    }
}
