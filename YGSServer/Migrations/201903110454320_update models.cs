namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.YGS_Cred", "CredType", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.YGS_Cred", "CredType");
        }
    }
}
