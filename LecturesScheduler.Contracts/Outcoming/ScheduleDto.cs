using System;

namespace LecturesScheduler.Contracts.Outgoing
{
    public class ScheduleDto
    {
        public string? LectureName { get; set; }

        public DateTime LectureTime { get; set; }
    }
}
