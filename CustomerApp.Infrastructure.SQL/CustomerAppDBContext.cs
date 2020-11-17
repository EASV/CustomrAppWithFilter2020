using System;
using System.Linq;
using CustomerApp.Core.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CustomerApp.Infrastructure.SQL
{
    public class CustomerAppDBContext: DbContext
    {
        public CustomerAppDBContext(
            DbContextOptions<CustomerAppDBContext> opt) : base(opt)
        {
        }

        public void DetachAll()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasKey(city => new {city.ZipCode});
            modelBuilder.Entity<City>()
                .Property(c => c.ZipCode)
                .ValueGeneratedNever();
            modelBuilder.Entity<Country>()
                .HasMany<City>(c => c.Cities)
                .WithOne(c => c.Country)
                .HasForeignKey(c => new {c.CountryId});
            modelBuilder.Entity<Address>()
                .HasOne(a => a.City)
                .WithMany(c => c.Addresses)
                .HasForeignKey(a => new {a.CityId});
        
        }
        
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Hat> Hats { get; set; }
        public DbSet<HatType> HatTypes { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Brand> Brands { get; set; }

        public DbSet<Address> Addresses { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Tourist> Tourists { get; set; }
        public DbSet<Country> Countries { get; set; }
    }
}