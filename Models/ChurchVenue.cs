using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class ChurchVenue
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VenueId { get; set; }    

        public string VenueName { get; set; }

        public string VenueDescription { get; set; }

        public string VenueCapacity { get; set; }

        public string PanoramaUrl { get; set; }
        public virtual ICollection<VenueImages> VenueImages { get; set; }
    }
}