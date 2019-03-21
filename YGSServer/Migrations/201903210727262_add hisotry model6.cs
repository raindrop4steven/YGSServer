namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addhisotrymodel6 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.YGS_History", "ApplyId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.YGS_History", "ApplyId", c => c.Int());
        }
    }
}
