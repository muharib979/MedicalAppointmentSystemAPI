using Core.Application.Commands;
using Core.Application.Queries;
using MediatR;
using MedicalAppointmentSystem.API.Controllers.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs;

namespace MedicalAppointmentSystem.API.Controllers
{
    [Route("api")]
    [ApiController]
    public class AppointmentController : BaseApiController
    {

        [HttpPost("save-appointment")]
        public async Task<IActionResult> SaveAppointment([FromBody] AppointmentDto dto)
        {
            return Ok(await _mediatr.Send(new CreateAppointmentCommand { Model = dto }));
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



    }
}

