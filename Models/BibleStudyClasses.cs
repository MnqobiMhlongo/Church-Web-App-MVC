using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class BibleStudyClasses
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int classId { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime DateTime { get; set; }
        public string classdecription { get; set; }
        public int venueId { get; set; }
        public string churchLeader {  get; set; }
    }
}