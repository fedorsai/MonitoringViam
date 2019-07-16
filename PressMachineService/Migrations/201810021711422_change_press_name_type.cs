namespace PressMachineServices.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_press_name_type : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Presses", "Name", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Presses", "Name", c => c.Int(nullable: false));
        }
    }
}
