package mocking.emis.appointments

import mocking.emis.models.Appointment
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder

data class GetAppointmentsResponseModel(
        var appointmentsFromDateTime: String,
        var appointments: List<Appointment> = emptyList(),
        var locations: List<Location> = emptyList(),
        var SessionHolders: List<SessionHolder> = emptyList(),
        var sessions: List<Session> = emptyList()
)
