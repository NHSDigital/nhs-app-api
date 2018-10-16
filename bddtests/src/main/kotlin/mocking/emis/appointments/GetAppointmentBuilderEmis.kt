package mocking.emis.appointments

import constants.EmisResponseCode
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.models.Appointment
import mocking.emis.models.ExceptionResponse
import mocking.emis.models.Location
import mocking.emis.models.Session
import mocking.emis.models.SessionHolder
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.MyAppointmentsFacade
import models.Patient
import org.apache.http.HttpStatus

class GetAppointmentBuilderEmis(configuration: EmisConfiguration?, patient: Patient,
                                fetchPreviousAppointments: Boolean = false)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointments"), IMyAppointmentsBuilder {

    init {
        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, patient.endUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, patient.sessionId)
                .andQueryParameter("userPatientLinkToken", patient.userPatientLinkToken)
                .andQueryParameter("fetchPreviousAppointments", fetchPreviousAppointments.toString())
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {
        return responseErrorForbiddenService()
    }

    override fun respondWithUnknownException(): Mapping {
        val exceptionResponse = ExceptionResponse(EmisResponseCode.EXCEPTION,
                "Unknown Exception")
        return respondWithException(exceptionResponse)
    }

    private fun respondWithException(exceptionResponse: ExceptionResponse): Mapping {
        return respondWithBody(exceptionResponse, HttpStatus.SC_INTERNAL_SERVER_ERROR)
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
                    .andDelay(delayMillisecs)

        }
    }

    override fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping {
        return respondWithBody(
                GetAppointmentsResponseModel(
                        facade.appointmentsFromDateTime,
                        extractListOfAppointmentsFromFacade(facade),
                        extractLocationsFromFacade(facade),
                        extractCliniciansFromFacade(facade),
                        extractSessionsFromFacade(facade)
                )
        )
    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody(body, contentType = "application/json")
        }
    }

    private fun extractListOfAppointmentsFromFacade(facade: MyAppointmentsFacade): List<Appointment> {
        return facade.slots?.sessions?.flatMap { session ->
            session.slots.map { slot ->
                Appointment(
                        slot.slotId!!,
                        session.sessionId!!,
                        slot.startTime!!,
                        slot.endTime!!,
                        slotTypeName = slot.slotTypeName!!
                )
            }
        } ?: emptyList()
    }

    private fun extractLocationsFromFacade(facade: MyAppointmentsFacade): List<Location> {
        return facade.slots?.sessions?.map { session -> Location(session.locationid!!, session.location!!) }
                ?: emptyList()
    }

    private fun extractCliniciansFromFacade(facade: MyAppointmentsFacade): List<SessionHolder> {
        val cliniciansAcrossAllSessions = facade.slots?.sessions?.flatMap { session ->
            session.staffDetails.map { clinician ->
                SessionHolder(clinician.staffDetailsid!!, clinician.staffName!!)
            }
        } ?: emptyList()
        return cliniciansAcrossAllSessions.distinct()
    }

    private fun extractSessionsFromFacade(facade: MyAppointmentsFacade): List<Session> {
        return facade.slots?.sessions?.map { session ->
            Session(
                    session.sessionType!!,
                    session.sessionId!!,
                    session.locationid,
                    clinicianIds = session.staffDetails.map { staff -> staff.staffDetailsid!! }
            )
        } ?: emptyList()
    }
}
