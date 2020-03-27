using Facebook.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Classes
{
    public class UserRepository : IUser
    {
        AppDbContext db;
        public UserRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Delete(int Id)
        {
            ApplicationUser applicationUser = db.Users.Find(Id);
            db.Users.Remove(applicationUser);
            db.SaveChanges();
        }

        public IEnumerable<ApplicationUser> GetAll()
        {
            return db.Users.ToList();
        }

        public ApplicationUser GetById(int Id)
        {
            return db.Users.Find(Id);
        }

        public void Insert(ApplicationUser applicationUser)
        {
            db.Users.Add(applicationUser);
            db.SaveChanges();
        }

        public void Update(ApplicationUser applicationUser)
        {
            db.Entry(applicationUser).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
