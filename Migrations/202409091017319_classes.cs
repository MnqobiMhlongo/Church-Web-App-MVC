namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class classes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BibleStudyClasses",
                c => new
                    {
                        classId = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        classdecription = c.String(),
                        venueId = c.Int(nullable: false),
                        churchLeader = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.classId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.BibleStudyClasses", "CourseId", "dbo.Courses");
            DropIndex("dbo.BibleStudyClasses", new[] { "CourseId" });
            DropTable("dbo.BibleStudyClasses");
        }
    }
}
