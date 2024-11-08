namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class paid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VenueBookingEvents", "Paid", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.VenueBookingEvents", "Paid");
        }
    }
}
