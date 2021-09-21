namespace LecturesScheduler.Domain.Entities
{
    public class ScheduleEntity : Entity
    {
        public string? LecturerFirstName { get; set; }

        public string? LecturerLastName { get; set; }

        public string? LectureName { get; set; }

        public DateTime LectureTime { get; set; }
    }
}