package mocking.emis.appointments

import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder

class GetAppointmentSlotsMetaResponseModel(
        val locations: List<Location>,
        val sessionHolders: List<SessionHolder>,
        val sessions: List<Session>
)