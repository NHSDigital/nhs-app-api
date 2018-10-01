package mocking.emis.appointments

import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder

class GetAppointmentSlotsMetaResponseModel(
        val locations: ArrayList<Location>,
        val sessionHolders: ArrayList<SessionHolder>,
        val sessions: ArrayList<Session>
)