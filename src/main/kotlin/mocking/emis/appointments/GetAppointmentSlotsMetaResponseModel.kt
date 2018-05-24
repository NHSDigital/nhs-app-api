package mocking.emis.appointments

import mocking.emis.models.Location
import mocking.emis.models.SessionHolder
import mocking.emis.models.appointmentslots.MetaSession

class GetAppointmentSlotsMetaResponseModel(
        val locations: ArrayList<Location>,
        val sessionHolders: ArrayList<SessionHolder>,
        val sessions: ArrayList<MetaSession>)