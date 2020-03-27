using Facebook.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Classes
{
    public class LikeRepository : ILike
    {
        AppDbContext db;
        public LikeRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Delete(int Id)
        {
            Like like = db.Likes.Find(Id);
            db.Likes.Remove(like);
            db.SaveChanges();
        }

        public IEnumerable<Like> GetAll()
        {
            return db.Likes.ToList();
        }

        public Like GetById(int Id)
        {
            return db.Likes.Find(Id);
        }

        public void Insert(Like like)
        {
            db.Likes.Add(like);
            db.SaveChanges();
        }

        public void Update(Like like)
        {
            db.Entry(like).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
