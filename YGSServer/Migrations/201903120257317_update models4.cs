namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodels4 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.YGS_Apply", "UserId", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.YGS_Apply", "UserId", c => c.Int(nullable: false));
        }
    }
}
