using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Razor.Text;

namespace ChurchSystem.Models
{
    public class Events
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EventId { get; set; }

        public string Title { get; set; }

        public string Description { get; set; } 

        public DateTime dateTime { get; set; }

        public string Location { get; set; }

        public string Organizer { get; set; }

        public DateTime DatePosted { get; set; }

        public string Status { get; set; }

        public int Rating {get; set; }

        public string Comments { get; set; }  
    }
}