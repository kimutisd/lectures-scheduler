using LecturesScheduler.Contracts.Incoming;
using LecturesScheduler.Domain;
using LecturesScheduler.Services.Mapper;
using LecturesScheduler.Services.Reposiroty;
using System;

namespace LecturesScheduler.Services.Services
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleRepository _scheduleRepository;

        public ScheduleService(IScheduleRepository scheduleRepository)
        {
            _scheduleRepository = scheduleRepository;
        }

        public Schedule? GetScheduleByName(string firstName, string lastName)
        {
            return _scheduleRepository.GetScheduleByName(firstName, lastName)?.ToDomain();
        }

        public Schedule? GetScheduleById(Guid id)
        {
            return _scheduleRepository.GetScheduleById(id)?.ToDomain();
        }

        public Guid AddSchedule(LecturerScheduleDto schedule)
        {
            return _scheduleRepository.AddSchedule(schedule.ToEntity());
        }
    }
}
