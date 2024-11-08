using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class Announcement
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int announcement { get; set; }

        public Course Course { get; set; }
        public int CourseId { get; set; }

        public string Message { get; set; }

        public DateTime DateTimeSent { get; set; }

        public string author { get; set; }

    }
}