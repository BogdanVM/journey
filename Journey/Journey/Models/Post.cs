using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Journey.Models
{
    public class Post
    {
        [Key]
        public int PostId { get; set; }

        [Required(ErrorMessage = "Postarea trebuie sa aiba o descriere")]
        public string Description { get; set; }

        [NotMapped]
        public HttpPostedFileBase PhotoFile { get; set; }

        [Required(ErrorMessage = "Nu ati selectat o poza")]
        public string Photo{ get; set; }

        [Required(ErrorMessage = "Nu ati selectat o categorie")]
        public int CategoryId { get; set; }

        public string UserId { get; set; }
        public DateTime Date { get; set; }

        public ICollection<int> CommentIds { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }

        public virtual Category Category { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Comment> Comment { get; set; }
        public virtual ICollection<Album> Album { get; set; }
    }
}