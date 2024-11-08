namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class migt : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EventSurveys",
                c => new
                    {
                        SurveyId = c.Int(nullable: false, identity: true),
                        EventId = c.Int(nullable: false),
                        ContentReview = c.String(),
                        VenueReview = c.String(),
                        PresentersReview = c.String(),
                        LocationReview = c.String(),
                        SatisfactionReview = c.String(),
                        Recommend = c.String(),
                        AttendAgain = c.String(),
                        LengthAndSchedule = c.String(),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => t.SurveyId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.EventSurveys");
        }
    }
}
