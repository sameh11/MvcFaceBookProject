using Facebook.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Classes
{
    public class PostRepository : IPost
    {
        AppDbContext db;
        public PostRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Delete(int Id)
        {
            Post post = db.Posts.Find(Id);
            db.Posts.Remove(post);
            db.SaveChanges();
        }

        public IEnumerable<Post> GetAll()
        {
            return db.Posts.ToList();
        }

        public Post GetById(int Id)
        {
            return db.Posts.Find(Id);
        }

        public void Insert(Post post)
        {
            db.Posts.Add(post);
            db.SaveChanges();
        }

        public void Update(Post post)
        {
            db.Entry(post).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
