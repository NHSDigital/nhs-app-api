package mocking.emis.appointments

import constants.EmisResponseCode
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.appointments.helpers.GetAppointmentHelper
import mocking.emis.models.ExceptionResponse
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

    override fun responseWithExceptionWhenServiceUnavailable(): Mapping {
        return respondWithBody("Service unavailable", HttpStatus.SC_SERVICE_UNAVAILABLE)
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
                        GetAppointmentHelper.extractListOfAppointmentsFromFacade(facade),
                        GetAppointmentHelper.extractLocationsFromFacade(facade),
                        GetAppointmentHelper.extractCliniciansFromFacade(facade),
                        GetAppointmentHelper.extractSessionsFromFacade(facade)
                )
        )
    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody(body, contentType = "application/json")
        }
    }

    override fun respondWithCorrupted(facade: MyAppointmentsFacade): Mapping {
        val mapping = respondWithSuccess(facade)
        return respondWith(HttpStatus.SC_OK) {
            andBody(mapping.response!!.body!!.replace(">", "|").replace("}", "|"), contentType = "application/json")
        }

    }
}
