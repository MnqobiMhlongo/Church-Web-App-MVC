using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class Question
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Text { get; set; }
        public string Answer { get; set; }
        public string Answer2 { get; set; }
        public int QuizId { get; set; }
        public string CorrectAnswer { get; set; }

        // Navigation property
        public virtual Quiz Quiz { get; set; }

        [NotMapped]
        public List<string> ShuffledAnswers { get; set; }
    }
}