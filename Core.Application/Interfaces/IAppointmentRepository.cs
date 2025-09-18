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
        Task<bool> UpdateAppointmentAsync(int appointmentId,AppointmentDto model);
        Task<bool> DeleteAppointmentAsync(int appointmentId);
        Task<List<AppointmentListDto>> GetAppointmentsAsync();
        Task<List<PatientDto>> GetPatientsAsync();
        Task<List<DoctorDto>> GetDoctorsAsync();
        Task<List<MedicineDto>> GetMedicinesAsync();
    }
}
