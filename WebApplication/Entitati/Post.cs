using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Entitati
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        public string Title {get;set;}

        public string Text { get; set; }

        public Users Users { get; set; }

        public int UserId { get; set; }
    }
}
