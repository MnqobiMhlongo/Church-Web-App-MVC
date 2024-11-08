using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class ChurchFunds
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FundId { get; set; }

        public string FundName { get; set; }
    }
}