using LecturesScheduler.Contracts.Outgoing;
using LecturesScheduler.Domain;
using LecturesScheduler.Domain.Entities;

namespace LecturesScheduler.Services.Mapper
{
    public static class ScheduleMapper
    {
        public static ScheduleDto ToDto(this Schedule from)
        {
            return new ScheduleDto { LectureName = from.LectureName, LectureTime = from.LectureTime };
        }
        
        public static Schedule ToDomain(this ScheduleEntity from)
        {
            return new Schedule { LectureName = from.LectureName, LectureTime = from.LectureTime };
        }
    }
}
