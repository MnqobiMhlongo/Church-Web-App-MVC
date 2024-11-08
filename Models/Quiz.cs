using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class Quiz
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuizID { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CourseId { get; set; }
        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
        public object Id { get; internal set; }

    }
}