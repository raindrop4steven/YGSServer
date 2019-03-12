namespace YGSServer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatemodels : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.YGS_Apply",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        OutName = c.String(),
                        Desc = c.String(),
                        CredType = c.String(),
                        ApplyDate = c.DateTime(nullable: false),
                        OutUsers = c.String(),
                        ApplyAtt = c.String(),
                        OutDate = c.DateTime(),
                        SignStatus = c.String(),
                        AfterAtt = c.String(),
                        ApplyStatus = c.String(),
                        CheckOpinion = c.String(),
                        NextStep = c.String(),
                        CreateTime = c.DateTime(),
                        UpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.YGS_Att",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Path = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.YGS_Cred",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        TradeCode = c.String(),
                        CredType = c.String(),
                        Name = c.String(),
                        Sex = c.Int(nullable: false),
                        CredUnit = c.String(),
                        CredDate = c.DateTime(),
                        ValidDate = c.DateTime(),
                        ValidStatus = c.String(),
                        CreateTime = c.DateTime(),
                        UpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.YGS_User",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Sex = c.Int(nullable: false),
                        Location = c.String(),
                        BirthDay = c.DateTime(),
                        CredNo = c.String(),
                        Unit = c.String(),
                        Depart = c.String(),
                        Duty = c.String(),
                        Level = c.String(),
                        CreateTime = c.DateTime(),
                        UpdateTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.YGS_User");
            DropTable("dbo.YGS_Cred");
            DropTable("dbo.YGS_Att");
            DropTable("dbo.YGS_Apply");
        }
    }
}
