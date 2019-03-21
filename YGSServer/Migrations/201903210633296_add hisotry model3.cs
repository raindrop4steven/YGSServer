namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addhisotrymodel3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.YGS_History", "HisotryId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.YGS_History", "HisotryId", c => c.Int());
        }
    }
}
