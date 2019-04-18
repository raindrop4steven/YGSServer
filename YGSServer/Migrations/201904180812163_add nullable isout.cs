namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnullableisout : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.YGS_History", "IsOut", c => c.Boolean());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.YGS_History", "IsOut", c => c.Boolean(nullable: false));
        }
    }
}
