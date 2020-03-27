using Facebook.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Classes
{
    public class FriendRequestRepository : IFriendRequest
    {
        AppDbContext db;
        public FriendRequestRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Delete(int Id)
        {
            FriendRequest friendRequest = db.FriendRequests.Find(Id);
            db.FriendRequests.Remove(friendRequest);
            db.SaveChanges();
        }

        public IEnumerable<FriendRequest> GetAll()
        {
            return db.FriendRequests.ToList();
        }

        public FriendRequest GetById(int Id)
        {
            return db.FriendRequests.Find(Id);
        }

        public void Insert(FriendRequest friendRequest)
        {
            db.FriendRequests.Add(friendRequest);
            db.SaveChanges();
        }

        public void Update(FriendRequest friendRequest)
        {
            db.Entry(friendRequest).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
