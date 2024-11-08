namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stringonlayout : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.VenueBookingEvents", "layoutid", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.VenueBookingEvents", "layoutid", c => c.Int(nullable: false));
        }
    }
}
