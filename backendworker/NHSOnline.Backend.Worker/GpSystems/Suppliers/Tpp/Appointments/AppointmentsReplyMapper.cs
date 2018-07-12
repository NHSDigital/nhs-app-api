using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentsReplyMapper
    {
        AppointmentsResponse Map(ViewAppointmentsReply viewAppointmentsReply);
    }
    public class AppointmentsReplyMapper : IAppointmentsReplyMapper
    {
        private readonly IAppointmentMapper _mapper;

        public AppointmentsReplyMapper(IAppointmentMapper mapper)
        {
            _mapper = mapper;
        }
        public AppointmentsResponse Map(ViewAppointmentsReply viewAppointmentsReply)
        {
            var appointments = _mapper.Map(viewAppointmentsReply.Appointments);
            var response = new AppointmentsResponse { Appointments = appointments };

            return response;
            
        }
    }
}
