using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class VenueChecklist
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int VenueId { get; set;}
        public ChurchVenue ChurchVenue { get; set;}

        public bool Cleaned { get; set; }
        public bool SoundEquipemnt { get; set; }
        public bool Chairs { get; set; }
        public bool Lights { get; set; }
        public bool Tents { get; set; }
        public bool SafetyEquipment { get; set; }

    }
}