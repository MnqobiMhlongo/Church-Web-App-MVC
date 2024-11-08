using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class BookLoan
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int bookloanId { get; set; }
        public int BookId { get; set; }
        public Books Books { get; set; }
        public string username { get; set; }
        public string handover { get; set; }
        public bool signedout {  get; set; }
        public DateTime reservedate { get; set; }
        public DateTime? signedDate { get; set; }
        public DateTime expectedReturn { get; set; }
        public DateTime? actualReturn { get; set; }
        public string signoutImageUrl { get; set; }
        public string signreturnImageUrl { get; set; }
        public double Bookfine { get; set; }
        public string status { get; set; }
    }
}