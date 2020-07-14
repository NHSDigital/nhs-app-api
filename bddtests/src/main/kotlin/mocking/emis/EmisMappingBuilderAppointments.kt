package mocking.emis

import mocking.emis.appointments.AppointmentSlotsBuilderEmis
import mocking.emis.appointments.AppointmentSlotsMetaBuilderEmis
import mocking.emis.appointments.BookAppointmentsBuilderEmis
import mocking.emis.appointments.DeleteAppointmentsBuilderEmis
import mocking.emis.appointments.GetAppointmentBuilderEmis
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentMappingBuilder
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mockingFacade.appointments.BookAppointmentSlotFacade
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import java.time.ZonedDateTime

class EmisMappingBuilderAppointments(private var configuration: EmisConfiguration?) : IAppointmentMappingBuilder {

    override fun viewMyAppointmentsRequest(patient: Patient, appointmentType: IMyAppointmentsBuilder.AppointmentType):
            IMyAppointmentsBuilder = GetAppointmentBuilderEmis(
            configuration,
            patient.endUserSessionId,
            patient.sessionId,
            patient.userPatientLinkToken)

    override fun viewMyAppointmentsRequestViaProxy(
            patient: Patient,
            actingOnBehalfOf: Patient,
            appointmentType: IMyAppointmentsBuilder.AppointmentType):
            IMyAppointmentsBuilder = GetAppointmentBuilderEmis(
            configuration,
            patient.endUserSessionId,
            patient.sessionId,
            actingOnBehalfOf.userPatientLinkToken)

    override fun appointmentSlotsRequest(patient: Patient,
                                         fromDateTime: ZonedDateTime?,
                                         toDateTime: ZonedDateTime?) = AppointmentSlotsBuilderEmis(
            configuration!!,
            patient,
            fromDateTime,
            toDateTime)

    fun appointmentSlotsMetaRequest(patient: Patient,
                                    sessionStartDate: ZonedDateTime? = null,
                                    sessionEndDate: ZonedDateTime? = null) = AppointmentSlotsMetaBuilderEmis(
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
