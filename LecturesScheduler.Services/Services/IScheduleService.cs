using LecturesScheduler.Contracts.Incoming;
using LecturesScheduler.Domain;
using System;

namespace LecturesScheduler.Services.Services
{
    public interface IScheduleService
    {
        Schedule GetScheduleByName(string firstName, string lastName);

        Guid AddSchedule(LecturerScheduleDto schedule);

        Schedule GetScheduleById(Guid id);
    }
}
