using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class ChurchVenueBooking
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
       public int BookingId { get; set; }

        public int VenueId { get; set; }
        public ChurchVenue ChurchVenue { get; set; }
        public string Username { get; set; }

        public DateTime BookingDateTime { get; set; }

        public bool Decor {  get; set; }

        public string EventType { get; set; }        
    }
}