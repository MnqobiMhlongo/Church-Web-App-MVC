namespace ChurchSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class quizdone : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.QuizAttempts",
                c => new
                    {
                        StudentQuizAnswerId = c.Int(nullable: false, identity: true),
                        StudentQuizResultId = c.Int(nullable: false),
                        QuestionId = c.Int(nullable: false),
                        SubmittedAnswer = c.String(),
                        CorrectAnswer = c.String(),
                        IsCorrect = c.Boolean(nullable: false),
                        user = c.String(),
                    })
                .PrimaryKey(t => t.StudentQuizAnswerId)
                .ForeignKey("dbo.Questions", t => t.QuestionId, cascadeDelete: true)
                .ForeignKey("dbo.StudentQuizResults", t => t.StudentQuizResultId, cascadeDelete: false)
                .Index(t => t.StudentQuizResultId)
                .Index(t => t.QuestionId);
            
            CreateTable(
                "dbo.StudentQuizResults",
                c => new
                    {
                        StudentQuizResultId = c.Int(nullable: false, identity: true),
                        StudentId = c.String(maxLength: 128),
                        QuizId = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StudentQuizResultId)
                .ForeignKey("dbo.Quizs", t => t.QuizId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.StudentId)
                .Index(t => t.StudentId)
                .Index(t => t.QuizId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuizAttempts", "StudentQuizResultId", "dbo.StudentQuizResults");
            DropForeignKey("dbo.StudentQuizResults", "StudentId", "dbo.AspNetUsers");
            DropForeignKey("dbo.StudentQuizResults", "QuizId", "dbo.Quizs");
            DropForeignKey("dbo.QuizAttempts", "QuestionId", "dbo.Questions");
            DropIndex("dbo.StudentQuizResults", new[] { "QuizId" });
            DropIndex("dbo.StudentQuizResults", new[] { "StudentId" });
            DropIndex("dbo.QuizAttempts", new[] { "QuestionId" });
            DropIndex("dbo.QuizAttempts", new[] { "StudentQuizResultId" });
            DropTable("dbo.StudentQuizResults");
            DropTable("dbo.QuizAttempts");
        }
    }
}
