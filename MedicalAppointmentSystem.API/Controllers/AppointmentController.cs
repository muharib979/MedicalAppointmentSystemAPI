using Core.Application.Commands;
using Core.Application.Interfaces;
using Core.Application.Queries;
using MedicalAppointmentSystem.API.Controllers.Common;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace MedicalAppointmentSystem.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AppointmentController : BaseApiController
    {
        private readonly IAppointmentRepository _repository;
        private readonly IEmailService _emailService;
        public AppointmentController(IAppointmentRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        [HttpPost("save-appointment")]
        public async Task<IActionResult> SaveAppointment([FromBody] AppointmentDto dto)
        {
            var result = await _mediatr.Send(new CreateAppointmentCommand { Model = dto });

            if (!result.IsSuccess)
                return BadRequest("Failed to save appointment");

            var appointment = await _repository.GetAppointmentByIdAsync(result.AppointmentId);

            if (appointment == null)
                return NotFound("Appointment details not found.");

            await _emailService.SendAppointmentEmailAsync(appointment);

            return Ok(new
            {
                message = "Appointment saved & email sent",
                appointmentId = result.AppointmentId,
                appointment = appointment
            });
        }

        [HttpPut("appointments/{id}")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] AppointmentDto dto)
        {
            if (dto == null)
                return BadRequest("Appointment data is required.");

            var result = await _mediatr.Send(new UpdateAppointmentCommand
            {
                AppointmentId = id,
                Model = dto
            });

            return Ok(result);
        }

        [HttpDelete("appointments/{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid Appointment Id.");

            var result = await _mediatr.Send(new DeleteAppointmentCommand { AppointmentId = id });

            if (!result)
                return NotFound($"Appointment with Id {id} not found.");

            return Ok(new { Message = $"Appointment {id} deleted successfully." });
        }

        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments()
        {

            return Ok(await _mediatr.Send(new GetAppointmentsQuery()));
        }

        [HttpGet("appointments/{id}")]
        public async Task<IActionResult> GetAppointmentById(int id)
        {
            var result = await _mediatr.Send(new GetAppointmentByIdQuery(id));
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("get-all-patients")]
        public async Task<IActionResult> GetPatients()
        {

            return Ok(await _mediatr.Send(new GetPatientsQuery()));
        }

        [HttpGet("get-all-doctors")]
        public async Task<IActionResult> GetDoctors()
        {

            return Ok(await _mediatr.Send(new GetDoctorsQuery()));
        }

        [HttpGet("get-all-medicines")]
        public async Task<IActionResult> GetMedicines()
        {

            return Ok(await _mediatr.Send(new GetMedicinesQuery()));
        }



    }
}

