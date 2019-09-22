using LecturesScheduler.Domain.Entities;
using System;

namespace LecturesScheduler.Services.Reposiroty
{
    public interface IScheduleRepository
    {
        ScheduleEntity GetScheduleByName(string firstName, string lastName);

        Guid AddSchedule(ScheduleEntity scheduleEntity);

        ScheduleEntity GetScheduleById(Guid id);
    }
}
