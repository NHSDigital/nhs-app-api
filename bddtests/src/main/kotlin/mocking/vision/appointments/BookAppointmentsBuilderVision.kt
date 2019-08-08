package mocking.vision.appointments

import constants.ErrorResponseCodeVision
import mocking.JSonXmlConverter
import mocking.gpServiceBuilderInterfaces.appointments.IBookAppointmentsBuilder
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionMappingBuilder
import mocking.vision.helpers.VisionConstantsHelper
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.Appointment
import mocking.vision.models.appointments.PatientVision
import mocking.vision.models.appointments.Slot
import mockingFacade.appointments.BookAppointmentSlotFacade
import models.Patient
import org.apache.http.HttpStatus
import java.time.Duration

class BookAppointmentsBuilderVision(patient: Patient, request: BookAppointmentSlotFacade)
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
                patient.patientId)
        requestBuilder
                .andHeader(contentTypeHeader, contentTypeValue)
                .andBody(userSession.rosuAccountId, "contains")
                .andBody(userSession.apiKey, "contains")
                .andBody(userSession.odsCode, "contains")
                .andBody(userSession.accountId, "contains")
                .andBody(userSession.provider, "contains")
                .andBody(serviceDefinition.name, "contains")
                .andBody(JSonXmlConverter.wrapAroundXmlTag("vision:patientId", userSession.patientId), "contains")
                .andBody(JSonXmlConverter.wrapAroundXmlTag("vision:slotId", request.slotId.toString()), "contains")

        val reason = request.bookingReason
        reason?.let { reasonString ->
            requestBuilder.andBody(JSonXmlConverter.wrapAroundXmlTag("vision:reason", reasonString), "contains")
        }

    }

    private val successBookingResponse = Appointment(patient = PatientVision(id = patient.patientId),
            slot = Slot(id = request.slotId.toString(), reason = request.bookingReason))

    override fun withDelay(delayMilliseconds: Duration): BookAppointmentsBuilderVision {
        delayMillisecs = delayMilliseconds.toMillis().toInt()
        return this
    }

    override fun respondWithSuccess(): Mapping {
        val response = JSonXmlConverter.toXML(successBookingResponse, true)
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionResponse(response, serviceDefinition)).build()
        }
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWithCorruptedContent(serviceDefinition, "")
    }

    override fun respondWithConflictException(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                    serviceDefinition,
                    ErrorResponseCodeVision.APPOINTMENT_SLOT_NOT_BOOKED_TO_CURRENT_USER)).build()
        }
    }

    override fun respondWithBookingLimitException(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                    serviceDefinition,
                    ErrorResponseCodeVision.APPOINTMENT_BOOKING_LIMIT_REACHED)).build()
        }
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWithUnknownVisionError(serviceDefinition)
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return respondVisionErrorWhenServiceNotEnabled(serviceDefinition)
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                    serviceDefinition,
                    ErrorResponseCodeVision.APPOINTMENT_SLOT_NOT_FOUND)).build()
        }
    }

    override fun respondWithExceptionWhenRequiredFieldMissing(): Mapping {
        throw NotImplementedError("Not implemented for this GP system")
    }

    override fun respondWithExceptionWhenInThePast(): Mapping {
        // VISION ALLOWS To book appointments that are in past
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody("").build()
        }
    }

    override fun respondWithExceptionWhenBeforePracticeDefinedDays(): Mapping {
        throw NotImplementedError("Not implemented for this GP system")
    }

    override fun respondWithExceptionWhenAfterPracticeDefinedDays(): Mapping {
        throw NotImplementedError("Not implemented for this GP system")
    }
}