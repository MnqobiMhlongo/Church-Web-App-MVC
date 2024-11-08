using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class StudentQuiz
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int QuizID { get; set; }
        public Quiz Quiz { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }
    }
}