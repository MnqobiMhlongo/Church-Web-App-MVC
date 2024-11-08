using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class classAttendance
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int attendeeId{ get; set; }

        public int classId { get; set; }

        public string attendeeImagedata { get; set; }

        public string Name { get; set; }

    }
}