namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateusers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YGS_History", "ApplyId", c => c.Int(nullable: false));
            AddColumn("dbo.YGS_History", "SignNation", c => c.String());
            AddColumn("dbo.YGS_History", "IsOut", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.YGS_History", "IsOut");
            DropColumn("dbo.YGS_History", "SignNation");
            DropColumn("dbo.YGS_History", "ApplyId");
        }
    }
}
