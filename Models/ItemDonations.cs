using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class ItemDonations
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DonationId { get; set; }

        public string DonorName { get; set; }

        public string Item { get; set; }

        public string itemDescription { get; set; }

        public string Quantity { get; set; }

        public string ImageUrl { get; set; }

        public double Price { get; set; }

    }
}