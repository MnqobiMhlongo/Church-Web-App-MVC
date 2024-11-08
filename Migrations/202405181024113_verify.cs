namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class verify : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Attendances", "verification", c => c.String());
            AddColumn("dbo.Attendances", "attended", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Attendances", "attended");
            DropColumn("dbo.Attendances", "verification");
        }
    }
}
