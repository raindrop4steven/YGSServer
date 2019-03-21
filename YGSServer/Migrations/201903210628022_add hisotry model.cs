namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addhisotrymodel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.YGS_History",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SignNo = c.String(),
                        SignTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.YGS_History");
        }
    }
}
