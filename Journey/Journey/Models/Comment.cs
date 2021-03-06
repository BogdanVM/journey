﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Journey.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Comentariul nu poate sa fie gol")]
        public string Content { get; set; }

        public DateTime Date { get; set; }
        public string UserId { get; set; }

        public int PostId { get; set; }

        public virtual Post Post { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}