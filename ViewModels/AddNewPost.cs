using Facebook.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Facebook.Models.Repository.Classes;
using Microsoft.AspNetCore.Identity;

namespace Facebook.ViewModels
{
    public class AddNewPost
    {
        public DateTime? Date { get; set; }
        public int? Likes { get; set; }
        public IFormFile Photo { get; set; }
        public string Text { get; set; }
        public bool? IsDeleted { get; set; }
        public Feeling Feel { get; set; }
        public string UserId { get; set; }
        public ApplicationUser user { get; set; }
        private PostRepository Posts { get; set; }
        public IEnumerable<Post> mylist { get; set; }

        public IEnumerable<ApplicationUser> friendRequests { get; set; }
        public IEnumerable<ApplicationUser> friends { get; set; }

    }
}
