using LecturesScheduler.Domain;

namespace LecturesScheduler.Services.Services
{
    public interface IScheduleService
    {
        Schedule GetScheduleByName(string firstName, string lastName);
    }
}
