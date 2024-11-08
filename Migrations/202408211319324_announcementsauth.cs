namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class announcementsauth : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Announcements", "author", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Announcements", "author");
        }
    }
}
