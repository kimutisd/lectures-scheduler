using LecturesScheduler.Contracts.Outgoing;
using LecturesScheduler.Services.Mapper;
using LecturesScheduler.Services.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.Web.Http;

namespace LecturesScheduler.WebApi.Controllers
{
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("schedule")]
        [ProducesResponseType(typeof(ScheduleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetScheduleByName([FromUri] string lecturerFirstName, string lecturerLastName)
        {
            if(string.IsNullOrEmpty(lecturerFirstName) || string.IsNullOrEmpty(lecturerLastName))
            {
                return BadRequest("Lecturer first and last name must be provided");
            }

            var schedule = _scheduleService.GetScheduleByName(lecturerFirstName, lecturerLastName);

            if(schedule == null)
            {
                return NotFound($"No lectures found for {lecturerFirstName} {lecturerLastName}");
            }

            return Ok(schedule.ToDto());
        }
    }
}