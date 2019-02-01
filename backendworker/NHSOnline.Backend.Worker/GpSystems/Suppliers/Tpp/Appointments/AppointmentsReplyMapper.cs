using System.Linq;
﻿using NHSOnline.Backend.Worker.GpSystems.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IAppointmentsReplyMapper
    {
        AppointmentsResponse Map(ViewAppointmentsReply viewPastAppointmentsReply, ViewAppointmentsReply viewUpcomingAppointmentsReply);
    }
    public class AppointmentsReplyMapper : IAppointmentsReplyMapper
    {
        private readonly IAppointmentMapper _mapper;

        public AppointmentsReplyMapper(IAppointmentMapper mapper)
        {
            _mapper = mapper;
        }
        
        public AppointmentsResponse Map(ViewAppointmentsReply viewPastAppointmentsReply, ViewAppointmentsReply viewUpcomingAppointmentsReply)
        {
            var tppPastAppointments = _mapper.Map(viewPastAppointmentsReply?.Appointments);
            var tppUpcomingAppointments = _mapper.Map(viewUpcomingAppointmentsReply?.Appointments);
            var combinedAppointments = tppPastAppointments.Concat(tppUpcomingAppointments).ToList();
            
            var pastAppointments = combinedAppointments
                .Where(x => x is PastAppointment).Cast<PastAppointment>();
            var upcomingAppointments = combinedAppointments
                .Where(x => x is UpcomingAppointment).Cast<UpcomingAppointment>();
            
            var response = new AppointmentsResponse { 
                PastAppointments = pastAppointments,
                UpcomingAppointments = upcomingAppointments,
                PastAppointmentsEnabled = true };

            return response;
        }
    }
}
