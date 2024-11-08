using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class EventDecorLayOuts
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Layoutid { get; set; }
        public string layoutName { get; set; }
        public string layoutImageUrl { get; set; }

    }
}