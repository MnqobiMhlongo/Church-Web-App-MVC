using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class BookWishList
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int wishItemId { get; set; }

        public string username { get; set; }
        public int bookid { get; set; }
    }
}