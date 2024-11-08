namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imagesdelonCV : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ChurchVenues", "ImageUrl");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChurchVenues", "ImageUrl", c => c.String());
        }
    }
}
