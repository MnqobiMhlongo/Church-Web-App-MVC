namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Rating : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "Rating", c => c.Int(nullable: false));
            AddColumn("dbo.Events", "Comments", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "Comments");
            DropColumn("dbo.Events", "Rating");
        }
    }
}
