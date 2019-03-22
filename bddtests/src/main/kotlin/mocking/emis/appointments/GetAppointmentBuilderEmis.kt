package mocking.emis.appointments

import constants.DateTimeFormats
import mocking.GsonFactory
import mocking.emis.EmisConfiguration
import mocking.emis.EmisMappingBuilder
import mocking.emis.HEADER_API_END_USER_SESSION_ID
import mocking.emis.HEADER_API_SESSION_ID
import mocking.emis.appointments.helpers.GetAppointmentHelper
import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mockingFacade.appointments.MyAppointmentsFacade
import models.Patient
import org.apache.http.HttpStatus
import java.time.ZonedDateTime
import java.time.format.DateTimeFormatter
import java.util.*


class GetAppointmentBuilderEmis(configuration: EmisConfiguration?, patient: Patient)
    : EmisMappingBuilder(configuration, method = "GET", relativePath = "/appointments"), IMyAppointmentsBuilder {

    init {
        val fetchPreviousAppointments = true
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.dateWithoutTimeFormat)
        val currentDateMinusOneYear = ZonedDateTime.now().minusYears(1)
                .withZoneSameInstant(TimeZone.getTimeZone("Europe/London").toZoneId())

        requestBuilder
                .andHeader(HEADER_API_END_USER_SESSION_ID, patient.endUserSessionId)
                .andHeader(HEADER_API_SESSION_ID, patient.sessionId)
                .andQueryParameter("UserPatientLinkToken", patient.userPatientLinkToken)
                .andQueryParameter("FetchPreviousAppointments", fetchPreviousAppointments.toString())
                .andQueryParameter(
                        "PreviousAppointmentsFromDate",
                        queryDateFormat.format(currentDateMinusOneYear)
                )
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return responseErrorForbiddenService()
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithEmisUnknownError()
    }

    private fun respondWithBody(body: Any, statusCode: Int = HttpStatus.SC_OK): Mapping {
        return respondWith(statusCode) {
            andJsonBody(body, GsonFactory.asPascal)
                    .andDelay(delayMillisecs)

        }
    }

    override fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping {
        val queryDateFormat = DateTimeFormatter.ofPattern(DateTimeFormats.backendDateTimeFormatWithTimezone)
        val currentDateMinusOneYear = ZonedDateTime.now().minusYears(1)
                .withZoneSameInstant(TimeZone.getTimeZone("Europe/London").toZoneId())

        return respondWithBody(
                GetAppointmentsResponseModel(
                        queryDateFormat.format(currentDateMinusOneYear),
                        GetAppointmentHelper.extractListOfAppointmentsFromFacade(facade.myAppointments!!),
                        GetAppointmentHelper.extractLocationsFromFacade(facade.myAppointments),
                        GetAppointmentHelper.extractCliniciansFromFacade(facade.myAppointments),
                        GetAppointmentHelper.extractSessionsFromFacade(facade.myAppointments)
                )
        )
    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andBody(body, contentType = "application/json")
        }
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWithCorruptedContent("<<randomtag/>>")
    }

    override fun respondWithGPServiceUnavailableException(): Mapping {
        return respondWithServiceUnavailable()
    }
}
