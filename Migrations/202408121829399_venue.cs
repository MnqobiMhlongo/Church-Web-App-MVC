namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class venue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChurchVenueBookings",
                c => new
                    {
                        BookingId = c.Int(nullable: false, identity: true),
                        VenueId = c.Int(nullable: false),
                        Username = c.String(),
                        BookingDateTime = c.DateTime(nullable: false),
                        Decor = c.Boolean(nullable: false),
                        EventType = c.String(),
                    })
                .PrimaryKey(t => t.BookingId)
                .ForeignKey("dbo.ChurchVenues", t => t.VenueId, cascadeDelete: true)
                .Index(t => t.VenueId);
            
            CreateTable(
                "dbo.ChurchVenues",
                c => new
                    {
                        VenueId = c.Int(nullable: false, identity: true),
                        VenueName = c.String(),
                        VenueDescription = c.String(),
                        ImageUrl = c.String(),
                        VenueCapacity = c.String(),
                    })
                .PrimaryKey(t => t.VenueId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChurchVenueBookings", "VenueId", "dbo.ChurchVenues");
            DropIndex("dbo.ChurchVenueBookings", new[] { "VenueId" });
            DropTable("dbo.ChurchVenues");
            DropTable("dbo.ChurchVenueBookings");
        }
    }
}
