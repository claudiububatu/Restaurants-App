using IPSMDB.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IPSMDB.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public DbSet<Person> Person { get; set; }
        public DbSet<FoodPlace> FoodPlace { get; set; }
        public DbSet<FoodPlaceCategory> FoodPlaceCategory { get; set; }
        public DbSet<Location> Location { get; set; }
        public DbSet<PersonFriendship> PersonFriendship { get; set; }
        public DbSet<CustomerReview> CustomerReview { get; set; }
        public DbSet<ServiceReview> ServiceReview { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PersonFriendship>()
                .HasKey(pf => new { pf.PersonId, pf.FriendId });
            modelBuilder.Entity<PersonFriendship>()
                .HasOne(pf => pf.Person)
                .WithMany(p => p.PersonFriendships)
                .HasForeignKey(pf => pf.PersonId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PersonFriendship>()
                .HasOne(pf => pf.Friend)
                .WithMany()
                .HasForeignKey(pf => pf.FriendId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CustomerReview>()
                .HasOne(cr => cr.Person)
                .WithMany(p => p.ReviewsWritten)
                .HasForeignKey(cr => cr.PersonId);
            modelBuilder.Entity<CustomerReview>()
                .HasOne(cr => cr.FoodPlace)
                .WithMany(fp => fp.ReviewsReceived)
                .HasForeignKey(cr => cr.FoodPlaceId);

            modelBuilder.Entity<ServiceReview>()
                .HasOne(sr => sr.FoodPlace)
                .WithMany(fp => fp.ReviewsWritten)
                .HasForeignKey(sr => sr.FoodPlaceId);
            modelBuilder.Entity<ServiceReview>()
                .HasOne(sr => sr.Person)
                .WithMany(p => p.ReviewsReceived)
                .HasForeignKey(sr => sr.PersonId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
