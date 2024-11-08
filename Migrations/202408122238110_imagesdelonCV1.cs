namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class imagesdelonCV1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VenueImages", "Imagename", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.VenueImages", "Imagename");
        }
    }
}
