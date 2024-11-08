using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class QuizAttempt
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentQuizAnswerId { get; set; }
        public int StudentQuizResultId { get; set; }
        public int QuestionId { get; set; }
        public string SubmittedAnswer { get; set; }
        public string CorrectAnswer { get; set; }
        public bool IsCorrect { get; set; }
        public string user { get; set; }
        public virtual StudentQuizResult StudentQuizResult { get; set; }
        public virtual Question Question { get; set; }
    }
}