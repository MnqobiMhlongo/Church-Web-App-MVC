using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class Books
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BookId {  get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Status { get; set; }
        public string imageurl { get; set; }
        public DateTime published { get; set; }
    }
}