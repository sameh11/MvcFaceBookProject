using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Facebook.Models
{
    public class AppDbContext :IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) {

            //options.
        }

      //  public DbSet<ApplicationUser> UserTemps { get; set; }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<FriendRequest> FriendRequests { get; set; }

        public DbSet<Replay> Replays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<FriendRequest>()
             .HasKey(uc => new { uc.IdRecive, uc.IdSend });

            modelBuilder.Entity<FriendRequest>()
               .HasOne(c => c.Recive)
               .WithMany() // <-- one of this must be empty
               .HasForeignKey(pc => pc.IdRecive)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
               .HasOne(c => c.Send)
               .WithMany(p => p.FriendRequests)
               .HasForeignKey(pc => pc.IdSend);
        }

    }
}
