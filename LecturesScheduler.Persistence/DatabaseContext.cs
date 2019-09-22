using LecturesScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace LecturesScheduler.Persistence
{
    public class DatabaseContext : DbContext, IDatabaseContext
    {
        public DbSet<ScheduleEntity> Schedules { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
            ChangeTracker.Tracked += OnEntityTracked;
            ChangeTracker.StateChanged += OnEntityStateChanged;
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScheduleEntity>(x =>
            {
                x.HasKey(k => k.Id);
                x.Property(p => p.LecturerFirstName).IsRequired();
                x.Property(p => p.LecturerLastName).IsRequired();
                x.Property(p => p.LectureTime).IsRequired();
                x.Property(p => p.LectureName).IsRequired();
                x.Property(p => p.CreatedOn).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }

        private void OnEntityTracked(object sender, EntityTrackedEventArgs e)
        {
            if (!e.FromQuery && e.Entry.State == EntityState.Added && e.Entry.Entity is Entity entity)
            {
                entity.CreatedOn = DateTime.UtcNow;
            }
        }

        private void OnEntityStateChanged(object sender, EntityStateChangedEventArgs e)
        {
            if (e.NewState == EntityState.Modified && e.Entry.Entity is Entity entity)
            {
                entity.ModifiedOn = DateTime.UtcNow;
            }
        }
    }
}
