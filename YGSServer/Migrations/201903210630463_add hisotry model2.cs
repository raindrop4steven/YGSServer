namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addhisotrymodel2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YGS_History", "HisotryId", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.YGS_History", "HisotryId");
        }
    }
}
