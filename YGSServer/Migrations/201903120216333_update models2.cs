namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodels2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.YGS_Apply", "CreateTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
            AlterColumn("dbo.YGS_Apply", "UpdateTime", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.YGS_Apply", "UpdateTime", c => c.DateTime());
            AlterColumn("dbo.YGS_Apply", "CreateTime", c => c.DateTime());
        }
    }
}
