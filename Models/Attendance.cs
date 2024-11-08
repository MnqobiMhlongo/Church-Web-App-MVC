using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class Attendance
    {
        public int Id { get; set; }

        public int EventId { get; set; }
        public Events Events { get; set; }

        public string Attandee { get; set; }

        public string verification {  get; set; }

        public bool attended { get; set; }
    }
}