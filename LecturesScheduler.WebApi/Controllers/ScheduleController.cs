﻿using LecturesScheduler.Contracts;
using LecturesScheduler.Contracts.Incoming;
using LecturesScheduler.Contracts.Outgoing;
using LecturesScheduler.Services.Mapper;
using LecturesScheduler.Services.Services;
using LecturesScheduler.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace LecturesScheduler.WebApi.Controllers
{
    [ProducesResponseType(typeof(ErrorDetails), 500)]
    [ApiController]
    [ValidateRequest]
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
        public async Task<IActionResult> GetScheduleByName([FromQuery] string lecturerFirstName, string lecturerLastName)
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

        [Microsoft.AspNetCore.Mvc.HttpGet]
        [Microsoft.AspNetCore.Mvc.Route("schedule/{id}")]
        [ProducesResponseType(typeof(ScheduleDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetScheduleById([FromQuery] string id)
        {
            if (!Guid.TryParse(id, out var scheduleId))
            {
                return BadRequest("Lecturer first and last name must be provided");
            }

            var schedule = _scheduleService.GetScheduleById(scheduleId);

            if (schedule == null)
            {
                return NotFound($"No lectures with id {scheduleId}");
            }

            return Ok(schedule.ToDto());
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Microsoft.AspNetCore.Mvc.Route("schedule")]
        [ProducesResponseType(typeof(ScheduleDto), 200)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(400)]
        public IActionResult AddNewScheduleForLecturer([FromBody] LecturerScheduleDto lecturerSchduleDto)
        {
            var scheduleId = _scheduleService.AddSchedule(lecturerSchduleDto);

            return CreatedAtAction(nameof(GetScheduleById), new { id = scheduleId }, scheduleId);
        }
    }
}