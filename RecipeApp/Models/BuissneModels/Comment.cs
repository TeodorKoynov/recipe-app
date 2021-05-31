using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecipeApp.Models.BuissneModels
{
    public class Comment
    {
        public int CommentId { get; set; }

        [Required]
        public string Message { get; set; }

        public string WriterId { get; set; }

        public int RecipeId { get; set; }

        public Recipe Recipe { get; set; }

        public string WriterUsername { get; set; }

        public string WriterPhoto { get; set; }
    }
}
