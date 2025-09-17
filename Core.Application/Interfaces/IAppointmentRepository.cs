using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DTOs;

namespace Core.Application.Interfaces
{
    public interface IAppointmentRepository
    {
        Task<(bool IsSuccess, int AppointmentId)> SaveAppointmentAsync(AppointmentDto model);
    }
}
