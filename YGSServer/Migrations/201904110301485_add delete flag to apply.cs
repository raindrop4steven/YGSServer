namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddeleteflagtoapply : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YGS_Apply", "IsDelete", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.YGS_Apply", "IsDelete");
        }
    }
}
