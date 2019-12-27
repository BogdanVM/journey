using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Journey.Models
{
    public class Album
    {
        [Key]
        public int AlbumId { get; set; }

        public string Name { get; set; }
        
        public string UserId { get; set; }

        public virtual ICollection<Post> Post { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}