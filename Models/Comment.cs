using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChurchSystem.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        public string Text { get; set; }

        public string UserName { get; set; }

        public DateTime Timestamp { get; set; }

        // Foreign key to associate the comment with a sermon
        public int SermonId { get; set; }

        // Navigation property for the associated sermon
        public virtual OnlineSermons Sermon { get; set; }
    }
}