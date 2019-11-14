using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public interface IAppointmentTypeTransformingVisitor :
        IAppointmentsResultVisitor<Task>,
        IAppointmentSlotsResultVisitor<Task> {
    }

    public class AppointmentTypeTransformingVisitor : IAppointmentTypeTransformingVisitor
    {
        private readonly ILogger<AppointmentTypeTransformingVisitor> _logger;

        public AppointmentTypeTransformingVisitor(ILogger<AppointmentTypeTransformingVisitor> logger)
        {
            _logger = logger;
        }
        
        private static readonly IDictionary<string, string> SlotTypeTranslations = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
        {
            { "Default", "General" }
        };
        
        #region No-ops for unsuccessful results

        public Task Visit(AppointmentsResult.BadRequest result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentsResult.BadGateway result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentsResult.InternalServerError result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentsResult.Forbidden result)
        {
            return Task.CompletedTask;
        }
        
        public Task Visit(AppointmentSlotsResult.BadGateway result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentSlotsResult.InternalServerError result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(AppointmentSlotsResult.Forbidden result)
        {
            return Task.CompletedTask;
        }
        
        #endregion
        
        public Task Visit(AppointmentsResult.Success result)
        {
            TransformSlots(result.Response?.PastAppointments);
            TransformSlots(result.Response?.UpcomingAppointments);

            return Task.CompletedTask;
        }

        public Task Visit(AppointmentSlotsResult.Success result)
        {
            TransformSlots(result.Response?.Slots);

            return Task.CompletedTask;
        }
        
        internal virtual void TransformSlots(IEnumerable<ISlot> slots)
        {
            if (slots == null)
            {
                _logger.LogWarning("null slots");
                return;
            }
            
            foreach (var slot in slots)
            {
                TransformSlot(slot);
            }
        }

        internal virtual void TransformSlot(ISlot slot)
        {
            if (slot == null)
            {
                _logger.LogWarning("null slot");
                return;
            }
            
            slot.TypeFromGpSystem = slot.Type;

            if (SlotTypeTranslations.TryGetValue(slot.TypeFromGpSystem, out var translatedType))
            {
                slot.Type = translatedType;
            }
        }
    }
}