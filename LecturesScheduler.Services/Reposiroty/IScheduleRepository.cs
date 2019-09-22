using LecturesScheduler.Domain.Entities;

namespace LecturesScheduler.Services.Reposiroty
{
    public interface IScheduleRepository
    {
        ScheduleEntity GetScheduleByName(string firstName, string lastName);
    }
}
