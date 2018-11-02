package mocking.vision.appointments

import mocking.JSonXmlConverter
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionMappingBuilder
import mocking.vision.helpers.VisionConstantsHelper
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import org.apache.http.HttpStatus
import java.time.Duration

class BookAppointmentsBuilderVision (patient: Patient, request: BookAppointmentSlotFacade)
    : VisionMappingBuilder()
        , IBookAppointmentsBuilder {

    private val serviceDefinition = ServiceDefinition(
            VisionConstants.bookAppointmentName,
            VisionConstants.bookAppointmentVersion
    )

    init {
        val contentTypeHeader = "content-type"
        val contentTypeValue = "text/xml; charset=UTF-8"
        val userSession = VisionUserSession(
                patient.rosuAccountId,
                patient.apiKey,
                patient.odsCode,
                patient.patientId
        )
        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andBody(userSession.rosuAccountId, "contains")
                .andBody(userSession.apiKey, "contains")
                .andBody(userSession.odsCode, "contains")
                .andBody(userSession.accountId, "contains")
                .andBody(userSession.provider, "contains")
                .andBody(serviceDefinition.name, "contains")
                .andBody(userSession.patientId, "contains")
                .andBody(JSonXmlConverter.wrapAroundXmlTag("vision:slotId", request.slotId.toString()), "contains")

        val reason = request.bookingReason
        reason?.let { reasonString ->
            requestBuilder.andBody(JSonXmlConverter.wrapAroundXmlTag("vision:reason", reasonString), "contains")
        }

    }

    override fun withDelay(delayMilliseconds: Duration): BookAppointmentsBuilderVision {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionResponse("", serviceDefinition)).build()
        }
    }

    override fun respondWithUnavailableException(): Mapping {

        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithUnavailableException() is not yet implemented in " +
                        "BookAppointmentsBuilderVision")
    }

    override fun respondWithConflictException(): Mapping {

        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithConflictException() is not yet implemented in " +
                        "BookAppointmentsBuilderVision")
    }

    override fun respondWithUnknownException(): Mapping {

        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithUnknownException() is not yet implemented in " +
                        "BookAppointmentsBuilderVision")
    }

    override fun respondWithExceptionWhenNotEnabled(): Mapping {

        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithExceptionWhenNotEnabled() is not yet implemented in " +
                        "BookAppointmentsBuilderVision")
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {

        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithExceptionWhenNotAvailable() is not yet implemented in " +
                        "BookAppointmentsBuilderVision")
    }

    override fun respondWithExceptionWhenInThePast(): Mapping {

        throw UnsupportedOperationException(
                "Test Setup Incorrect: respondWithExceptionWhenInThePast() is not yet implemented in " +
                        "BookAppointmentsBuilderVision")
    }
}