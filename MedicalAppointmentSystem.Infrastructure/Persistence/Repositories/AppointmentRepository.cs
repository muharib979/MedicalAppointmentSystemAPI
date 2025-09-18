using Core.Application.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace MedicalAppointmentSystem.Infrastructure.Persistence.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly string _connectionString;

        public AppointmentRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }



        public async Task<(bool IsSuccess, int AppointmentId)> SaveAppointmentAsync(AppointmentDto model)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            await using var tran = await con.BeginTransactionAsync();
            try
            {

                var appointmentParam = new
                {
                    model.PatientId,
                    model.DoctorId,
                    model.AppointmentDate,
                    model.VisitType,
                    model.Diagnosis,
                    model.Notes
                };

                int appointmentId = await con.QuerySingleAsync<int>(
                    "SP_SaveAppointment",
                    param: appointmentParam,
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );


                foreach (var prescription in model.Prescriptions)
                {
                    var prescriptionParam = new
                    {
                        AppointmentId = appointmentId,
                        prescription.MedicineId,
                        prescription.Dosage,
                        prescription.StartDate,
                        prescription.EndDate,
                        prescription.Notes
                    };

                    await con.ExecuteAsync(
                        "SP_SavePrescriptionDetail",
                        param: prescriptionParam,
                        transaction: tran,
                        commandType: CommandType.StoredProcedure
                    );
                }

                await tran.CommitAsync();
                return (true, appointmentId);
            }
            catch (Exception ex)
            {
                await tran.RollbackAsync();
                throw new Exception($"Error saving appointment: {ex.Message}");
            }
        }



        public async Task<bool> UpdateAppointmentAsync(int appointmentId, AppointmentDto model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (model.Prescriptions == null)
                model.Prescriptions = new List<PrescriptionDto>();

            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            await using var tran = await con.BeginTransactionAsync();
            try
            {

                var appointmentParam = new
                {
                    AppointmentId = appointmentId,
                    model.PatientId,
                    model.DoctorId,
                    model.AppointmentDate,
                    model.VisitType,
                    model.Diagnosis,
                    model.Notes
                };

                await con.ExecuteAsync(
                    "SP_UpdateAppointment",
                    param: appointmentParam,
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );


                await con.ExecuteAsync(
                    "SP_DeletePrescriptionsByAppointmentId",
                    param: new { AppointmentId = appointmentId },
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );


                foreach (var prescription in model.Prescriptions)
                {
                    var prescriptionParam = new
                    {
                        AppointmentId = appointmentId,
                        prescription.MedicineId,
                        prescription.Dosage,
                        prescription.StartDate,
                        prescription.EndDate,
                        prescription.Notes
                    };

                    await con.ExecuteAsync(
                        "SP_SavePrescriptionDetail",
                        param: prescriptionParam,
                        transaction: tran,
                        commandType: CommandType.StoredProcedure
                    );
                }

                // Commit transaction
                await tran.CommitAsync();
                return true;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }


        public async Task<bool> DeleteAppointmentAsync(int appointmentId)
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

            await using var tran = await con.BeginTransactionAsync();
            try
            {
                await con.ExecuteAsync(
                    "SP_DeletePrescriptionsByAppointmentId",
                    param: new { AppointmentId = appointmentId },
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );

                
                int rows = await con.ExecuteAsync(
                    "SP_DeleteAppointment",
                    param: new { AppointmentId = appointmentId },
                    transaction: tran,
                    commandType: CommandType.StoredProcedure
                );

                await tran.CommitAsync();
                return rows > 0;
            }
            catch
            {
                await tran.RollbackAsync();
                throw;
            }
        }

        public async Task<List<AppointmentListDto>> GetAppointmentsAsync()
        {
            await using var con = new SqlConnection(_connectionString);
            await con.OpenAsync();

        
            var appointments = await con.QueryAsync<AppointmentListDto>(
                "SP_GetAppointments",
                commandType: CommandType.StoredProcedure
            );

            var appointmentList = appointments.ToList();

            foreach (var appointment in appointmentList)
            {
                var prescriptions = await con.QueryAsync<PrescriptionDto>(
                    "SP_GetPrescriptionsByAppointmentId",
                    new { AppointmentId = appointment.AppointmentId },
                    commandType: CommandType.StoredProcedure
                );

                appointment.Prescriptions = prescriptions.ToList();
            }

            return appointmentList;
        }
    }

}
