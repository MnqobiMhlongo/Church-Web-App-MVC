using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class EventProgram
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int programId { get; set; }

        public int EventId { get; set; }
        public Events Events { get; set; }

        public string activity { get; set; }

        public bool finished { get; set; }


    }
}