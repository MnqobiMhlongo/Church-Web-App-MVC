namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class venueevents : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.VenueBookingEvents",
                c => new
                    {
                        VenueEventId = c.Int(nullable: false, identity: true),
                        VenueId = c.Int(nullable: false),
                        BookingId = c.Int(nullable: false),
                        EventDescription = c.String(),
                        startTime = c.DateTime(nullable: false),
                        endTime = c.DateTime(nullable: false),
                        Attendees = c.Int(nullable: false),
                        tent = c.Boolean(nullable: false),
                        moreSeats = c.Boolean(nullable: false),
                        PostCleanup = c.Boolean(nullable: false),
                        equipment = c.Boolean(nullable: false),
                        layoutid = c.Int(nullable: false),
                        AddOns = c.Double(nullable: false),
                        TotalCost = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.VenueEventId)
                .ForeignKey("dbo.ChurchVenues", t => t.VenueId, cascadeDelete: false)
                .ForeignKey("dbo.ChurchVenueBookings", t => t.BookingId, cascadeDelete: false)
                .Index(t => t.VenueId)
                .Index(t => t.BookingId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VenueBookingEvents", "BookingId", "dbo.ChurchVenueBookings");
            DropForeignKey("dbo.VenueBookingEvents", "VenueId", "dbo.ChurchVenues");
            DropIndex("dbo.VenueBookingEvents", new[] { "BookingId" });
            DropIndex("dbo.VenueBookingEvents", new[] { "VenueId" });
            DropTable("dbo.VenueBookingEvents");
        }
    }
}
