using LecturesScheduler.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LecturesScheduler.Persistence
{
    public interface IDatabaseContext
    {
        DbSet<ScheduleEntity> Schedules { get; set; }

        int SaveChanges();
    }
}
