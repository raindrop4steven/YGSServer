namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodels2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YGS_Apply", "CheckOpinion", c => c.String());
            AddColumn("dbo.YGS_Apply", "NextStep", c => c.String());
            AlterColumn("dbo.YGS_Apply", "CreateTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.YGS_Apply", "UpdateTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.YGS_Apply", "UpdateTime", c => c.DateTime());
            AlterColumn("dbo.YGS_Apply", "CreateTime", c => c.DateTime());
            DropColumn("dbo.YGS_Apply", "NextStep");
            DropColumn("dbo.YGS_Apply", "CheckOpinion");
        }
    }
}
