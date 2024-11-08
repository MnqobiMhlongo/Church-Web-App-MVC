namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class coursematerial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CourseMaterials",
                c => new
                    {
                        MaterialId = c.Int(nullable: false, identity: true),
                        CourseId = c.Int(nullable: false),
                        Name = c.String(),
                        DatePosted = c.DateTime(nullable: false),
                        Description = c.String(),
                        FileType = c.String(),
                        FileUrl = c.String(),
                    })
                .PrimaryKey(t => t.MaterialId)
                .ForeignKey("dbo.Courses", t => t.CourseId, cascadeDelete: true)
                .Index(t => t.CourseId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CourseMaterials", "CourseId", "dbo.Courses");
            DropIndex("dbo.CourseMaterials", new[] { "CourseId" });
            DropTable("dbo.CourseMaterials");
        }
    }
}
