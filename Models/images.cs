using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class images
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Imageid { get; set; }

        public string imagedata { get; set; }

        public string name { get; set; }
    }
}