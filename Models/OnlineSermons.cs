using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.ComponentModel;

namespace ChurchSystem.Models
{
    public class OnlineSermons
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SermonId { get; set; }

        [DisplayName("Title")]
        public string SermonTitle { get; set; }
        [DisplayName("Description")]
        public string SermonDescription { get; set;}
        [DisplayName("Media")]
        public string FileType { get; set;}

        public string FileUrl { get; set;}

        public virtual ICollection<Comment> Comments { get; set; }
    }
}