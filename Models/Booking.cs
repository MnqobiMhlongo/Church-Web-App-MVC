using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace ChurchSystem.Models
{
    public class Booking
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string user { get; set; }

        public DateTime BookingDate { get; set; }

        public string Purpose { get; set; }

        public int leaderId { get; set; }
        public ChurchLeaders ChurchLeaders { get; set; }

        public bool isApproved { get; set; }

    }
}