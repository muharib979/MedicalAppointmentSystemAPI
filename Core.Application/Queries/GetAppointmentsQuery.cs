using Core.Application.Interfaces;
using MediatR;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Application.Queries
{
    public class GetAppointmentsQuery : IRequest<List<AppointmentListDto>>
    {

        public class Handler : IRequestHandler<GetAppointmentsQuery, List<AppointmentListDto>>
        {
            private readonly IAppointmentRepository _repository;
            public Handler(IAppointmentRepository repository)
            {
                _repository = repository;
            }
            public async Task<List<AppointmentListDto>> Handle(GetAppointmentsQuery request, CancellationToken cancellationToken)
            {
                var result = await _repository.GetAppointmentsAsync();
                return result;
            }
        }
    }
}
