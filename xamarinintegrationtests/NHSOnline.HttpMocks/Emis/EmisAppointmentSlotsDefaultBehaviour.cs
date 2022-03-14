using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.HttpMocks.Domain;
using NHSOnline.HttpMocks.Emis.Models;

namespace NHSOnline.HttpMocks.Emis
{
    public class EmisAppointmentSlotsDefaultBehaviour : IEmisAppointmentSlotsBehaviour
    {
        public IActionResult Behave()
        {
            return new JsonResult(new
            {
                Sessions = new ArrayList
                {
                    new AppointmentSession
                    {
                        SessionDate = "09-03-2022",
                        SessionId = "123",
                        Slots = new List<AppointmentSlots>
                        {
                            new()
                            {
                                SlotId = "123",
                                StartTime = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                                EndTime = DateTime.Now.AddDays(1).ToString(CultureInfo.InvariantCulture),
                                SlotTypeName = "Practice",
                                SlotTypeStatus = SlotTypeStatus.Practice,
                                TelephoneNumber = "1234567890",
                            }
                        }
                    }
                }
            });
        }
    }
}