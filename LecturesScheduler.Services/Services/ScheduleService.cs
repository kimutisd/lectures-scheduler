using LecturesScheduler.Domain;
using LecturesScheduler.Services.Mapper;
using LecturesScheduler.Services.Reposiroty;

namespace LecturesScheduler.Services.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public Schedule GetScheduleByName(string firstName, string lastName)
        {
            return _scheduleRepository.GetScheduleByName(firstName, lastName)?.ToDomain();
        }
    }
}
