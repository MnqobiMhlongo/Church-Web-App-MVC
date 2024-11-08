using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class Donations
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DonationId { get; set; }

        public string DonorName { get; set; }

        public double DonationAmount { get; set;}

        public DateTime DonationDate { get; set; }

        public int FundId { get; set; }
        public ChurchFunds ChurchFunds { get; set; }

    }
}