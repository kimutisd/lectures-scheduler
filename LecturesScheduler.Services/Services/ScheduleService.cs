using LecturesScheduler.Domain;
using System;

namespace LecturesScheduler.Services.Services
{
    public class ScheduleService : IScheduleService
    {
        public Schedule GetScheduleByName(string firstName, string lastName)
        {
            return new Schedule { LectureName = "TOP", LectureTime = DateTime.Now };
        }
    }
}
