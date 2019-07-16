namespace PressMachineServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Presses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PressOperationDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Position = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Power = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Speed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Temperature = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateInsert = c.DateTime(nullable: false),
                        Run = c.Boolean(nullable: false),
                        Press_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Presses", t => t.Press_Id)
                .Index(t => t.Press_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PressOperationDatas", "Press_Id", "dbo.Presses");
            DropIndex("dbo.PressOperationDatas", new[] { "Press_Id" });
            DropTable("dbo.PressOperationDatas");
            DropTable("dbo.Presses");
        }
    }
}
