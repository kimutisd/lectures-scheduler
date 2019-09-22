using LecturesScheduler.Contracts.Outgoing;
using LecturesScheduler.Domain;

namespace LecturesScheduler.Services.Mapper
{
    public static class ScheduleMapper
    {
        public static ScheduleDto ToDto(this Schedule from)
        {
            return new ScheduleDto { LectureName = from.LectureName, LectureTime = from.LectureTime };
        }        
    }
}
