using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class VenueImages
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageId { get; set; }

        public string Imagename { get; set; }
        public int VenueId { get; set; }
        public ChurchVenue ChurchVenue { get; set; }

        public string ImageUrl { get; set; }

        
    }
}