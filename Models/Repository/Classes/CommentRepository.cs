using Facebook.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Classes
{
    public class CommentRepository : IComment
    {
        AppDbContext db;
        public CommentRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Delete(int Id)
        {
            Comment comment = db.Comments.Find(Id);
            db.Comments.Remove(comment);
            db.SaveChanges();
        }

        public IEnumerable<Comment> GetAll()
        {
            return db.Comments.ToList();
        }

        public Comment GetById(int Id)
        {
            return db.Comments.Find(Id);
        }

        public void Insert(Comment obj)
        {
            db.Comments.Add(obj);
            db.SaveChanges();
        }

        public void Update(Comment comment)
        {
            db.Entry(comment).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
