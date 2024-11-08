using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class SendInvites
    {

        
            [Required(ErrorMessage = "At least one phone number is required.")]
            public List<string> PhoneNumbers { get; set; }

            [Required(ErrorMessage = "Message cannot be empty.")]
            public string Message { get; set; }
        

    }
}