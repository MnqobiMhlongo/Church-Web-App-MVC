namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class announcements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Announcements",
                c => new
                    {
                        announcement = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        Message = c.String(),
                        DateTimeSent = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.announcement)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Announcements", "CourseId", "dbo.Courses");
            DropIndex("dbo.Announcements", new[] { "CourseId" });
            DropTable("dbo.Announcements");
        }
    }
}
