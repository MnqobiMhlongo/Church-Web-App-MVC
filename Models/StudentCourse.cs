using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class StudentCourse
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentCourseID { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public DateTime enrollmentDate { get; set; }
        public bool completed { get; set; }
    }
}