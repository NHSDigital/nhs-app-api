package mocking.emis.appointments

import mocking.emis.models.AppointmentSession


data class GetAppointmentSlotsResponseModel(val sessions: ArrayList<AppointmentSession> = arrayListOf())