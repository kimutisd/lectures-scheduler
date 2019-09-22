using LecturesScheduler.Domain.Entities;
using LecturesScheduler.Persistence;
using System.Linq;

namespace LecturesScheduler.Services.Reposiroty
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly IDatabaseContext _databaseContext;

        public ScheduleRepository(IDatabaseContext databaseContext)
        {
            _databaseContext = databaseContext;
        }

        public ScheduleEntity GetScheduleByName(string firstName, string lastName)
        {
            return _databaseContext.Schedules
                .FirstOrDefault(schedule => 
                schedule.LecturerFirstName.Equals(firstName) &&
                schedule.LecturerLastName.Equals(lastName));
        }
    }
}
