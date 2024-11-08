using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class StudentQuizResult
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentQuizResultId { get; set; }
        public string StudentId { get; set; } // Link to ApplicationUser
        public int QuizId { get; set; }
        public int Score { get; set; }
        public string user { get; set; }
        public virtual ApplicationUser Student { get; set; }
        public virtual Quiz Quiz { get; set; }
        public string Passed { get; set; }

    }
}