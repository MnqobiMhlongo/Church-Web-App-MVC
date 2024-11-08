using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class CourseMaterial
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MaterialId { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }

        public string Name { get; set; }

        public DateTime DatePosted { get; set; }

        public string Description { get; set; }

        public string FileType { get; set; }

        public string FileUrl { get; set; }

    }
}