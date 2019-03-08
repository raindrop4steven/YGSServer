namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YGS_Apply", "UserId", c => c.Int(nullable: false));
            AddColumn("dbo.YGS_Apply", "ApplyStatus", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.YGS_Apply", "ApplyStatus");
            DropColumn("dbo.YGS_Apply", "UserId");
        }
    }
}
