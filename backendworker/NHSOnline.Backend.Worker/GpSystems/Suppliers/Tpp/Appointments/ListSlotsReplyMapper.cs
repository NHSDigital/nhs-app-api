using NHSOnline.Backend.Worker.Areas.Appointments.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models.Appointments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Appointments
{
    public interface IListSlotsReplyMapper
    {
        AppointmentSlotsResponse Map(ListSlotsReply listSlotsReply);
    }
    public class ListSlotsReplyMapper : IListSlotsReplyMapper
    {
        private readonly ISessionMapper _sessionMapper;

        public ListSlotsReplyMapper(ISessionMapper sessionMapper)
        {
            _sessionMapper = sessionMapper;
        }
        public AppointmentSlotsResponse Map(ListSlotsReply listSlotsReply)
        {
            var slots = _sessionMapper.Map(listSlotsReply.Sessions);

            var response = new AppointmentSlotsResponse
            {
                Slots = slots
            };

            return response;
        }
    }
}
