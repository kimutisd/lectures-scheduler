using System;
using System.ComponentModel.DataAnnotations;

namespace LecturesScheduler.Contracts.Incoming
{
    public class LecturerScheduleDto
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public DateTime LectureTime { get; set; }

        [Required]
        public string LectureName { get; set; }
    }
}
