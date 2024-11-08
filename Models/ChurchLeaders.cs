﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class ChurchLeaders
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int leaderId { get; set; }

        public string LeaderName { get; set; }

        public string Role { get; set; }

        public string ImageUrl { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }


    }
}