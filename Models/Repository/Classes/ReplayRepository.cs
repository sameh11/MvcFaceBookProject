using Facebook.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models.Repository.Classes
{
    public class ReplayRepository : IReplay
    {
        AppDbContext db;
        public ReplayRepository(AppDbContext db)
        {
            this.db = db;
        }
        public void Delete(int Id)
        {
            Replay replay = db.Replays.Find(Id);
            db.Replays.Remove(replay);
            db.SaveChanges();
        }

        public IEnumerable<Replay> GetAll()
        {
            return db.Replays.ToList();
        }

        public Replay GetById(int Id)
        {
            return db.Replays.Find(Id);
        }

        public void Insert(Replay replay)
        {
            db.Replays.Add(replay);
            db.SaveChanges();
        }

        public void Update(Replay replay)
        {
            db.Entry(replay).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}
