using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class VenueBookingEvent
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VenueEventId { get; set; }
        public int VenueId { get; set;}
        public ChurchVenue ChurchVenue { get; set; }
        public int BookingId { get; set; }
        public ChurchVenueBooking ChurchVenueBooking { get; set; }
        public string EventDescription { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public int Attendees { get; set; }
        public bool tent { get; set; }
        public bool moreSeats { get; set; } 
        public bool PostCleanup { get; set; }
        public bool equipment { get; set; }
        public string layoutid { get; set; }    
        public double AddOns { get; set; }
        public double TotalCost { get; set; }
        public bool Paid { get; set; }
    }
}