using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class EventSurvey
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SurveyId { get; set; }
        public int EventId { get; set; }
        public string ContentReview { get; set; }
        public string VenueReview { get; set; }
        public string PresentersReview { get; set; }
        public string LocationReview { get; set; }
        public string SatisfactionReview { get; set; }
        public string Recommend { get; set; }
        public string AttendAgain { get; set; }
        public string LengthAndSchedule { get; set; }
        public string Comment { get; set; }
    }
}