﻿using System.ComponentModel.DataAnnotations;

namespace AboutNow.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        [Required(ErrorMessage = "Continutul este obligatoriu")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int JournalId { get; set; }
        public virtual Journal Journal { get; set; }
    }

}
