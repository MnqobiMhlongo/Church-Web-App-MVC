namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class quizinitial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Answer = c.String(),
                        Answer2 = c.String(),
                        QuizId = c.Int(nullable: false),
                        CorrectAnswer = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Quizs", t => t.QuizId, cascadeDelete: true)
                .Index(t => t.QuizId);
            
            CreateTable(
                "dbo.Quizs",
                c => new
                    {
                        QuizID = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        CreatedAt = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.QuizID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Questions", "QuizId", "dbo.Quizs");
            DropIndex("dbo.Questions", new[] { "QuizId" });
            DropTable("dbo.Quizs");
            DropTable("dbo.Questions");
        }
    }
}
