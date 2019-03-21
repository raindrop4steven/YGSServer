namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addhisotrymodel5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YGS_History", "ApplyId", c => c.Int());
            DropColumn("dbo.YGS_History", "HisotryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.YGS_History", "HisotryId", c => c.Int());
            DropColumn("dbo.YGS_History", "ApplyId");
        }
    }
}
