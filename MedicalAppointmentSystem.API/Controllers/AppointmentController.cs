using Core.Application.Commands;
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
        
    }
}

