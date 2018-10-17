package mocking.emis

import mocking.emis.appointments.AppointmentSlotsBuilderEmis
import mocking.emis.appointments.AppointmentSlotsMetaBuilderEmis
import mocking.emis.appointments.BookAppointmentsBuilderEmis
import mocking.emis.appointments.DeleteAppointmentsBuilderEmis
import mocking.emis.appointments.GetAppointmentBuilderEmis
import mocking.emis.practices.PracticeSettingsBuilderEmis
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient

class EmisMappingBuilderAppointments(private var configuration: EmisConfiguration?) : IAppointmentMappingBuilder {

    fun practiceSettingsRequest(patient: Patient) = PracticeSettingsBuilderEmis(patient)

    override fun viewMyAppointmentsRequest(patient: Patient):
            IMyAppointmentsBuilder = GetAppointmentBuilderEmis(configuration, patient)

    override fun appointmentSlotsRequest(patient: Patient,
                                         fromDateTime: String?, toDateTime: String?) = AppointmentSlotsBuilderEmis(
            configuration!!,
            patient,
            fromDateTime,
            toDateTime)

    fun appointmentSlotsMetaRequest(patient: Patient,
                                    sessionStartDate: String? = null,
                                    sessionEndDate: String? = null) = AppointmentSlotsMetaBuilderEmis(
            configuration!!,
            patient.endUserSessionId,
            patient.sessionId,
            sessionStartDate,
            sessionEndDate,
            patient.userPatientLinkToken)

    override fun bookAppointmentSlotRequest(patient: Patient,
                                            request: BookAppointmentSlotFacade) =
            BookAppointmentsBuilderEmis(configuration!!, patient.endUserSessionId, patient.sessionId, request)

    override fun cancelAppointmentRequest(patient: Patient, request: CancelAppointmentSlotFacade)
            = DeleteAppointmentsBuilderEmis(configuration!!, patient, request)
}