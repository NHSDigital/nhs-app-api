package mocking.vision.appointments

import constants.ErrorResponseCodeVision
import mocking.JSonXmlConverter
import mocking.emis.models.AppointmentCancellationReason
import mocking.gpServiceBuilderInterfaces.appointments.ICancelAppointmentsBuilder
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionMappingBuilder
import mocking.vision.helpers.VisionConstantsHelper
import mocking.vision.helpers.VisionConstantsHelper.Companion.getBaseVisionResponse
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.Appointment
import mocking.vision.models.appointments.PatientVision
import mocking.vision.models.appointments.Slot
import mockingFacade.appointments.CancelAppointmentSlotFacade
import models.Patient
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus

class CancelAppointmentBuilderVision(patient: Patient, request: CancelAppointmentSlotFacade)
    : VisionMappingBuilder(method = "POST"), ICancelAppointmentsBuilder {

    private val serviceDefinition = ServiceDefinition(
            VisionConstants.cancelAppointmentsName,
            VisionConstants.cancelAppointmentsVersion
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
                .andBody(JSonXmlConverter.wrapAroundXmlTag(
                        "vision:slotId",
                        request.slotId.toString()
                ), "contains")
        val reasonId = retrieveMatchingReasonId(request.cancellationReason)

        reasonId?.let { rId ->
            requestBuilder.andBody(JSonXmlConverter.wrapAroundXmlTag(
                    "vision:reasonId",
                    rId
            ), "contains")
        }
    }

    private val successCancellationResponse = Appointment(patient = PatientVision(id = patient.patientId),
            slot = Slot(id = request.slotId.toString(), reason = request.cancellationReason))

    override fun respondWithSuccess(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            val response = JSonXmlConverter.toXML(successCancellationResponse, true)
            val serviceDefinition = ServiceDefinition(
                    VisionConstants.cancelAppointmentsName,
                    VisionConstants.cancelAppointmentsVersion)

            andXmlBody(getBaseVisionResponse(response, serviceDefinition))
        }
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            val response = JSonXmlConverter.toXML(successCancellationResponse, true)
            val serviceDefinition = ServiceDefinition(
                    VisionConstants.cancelAppointmentsName,
                    VisionConstants.cancelAppointmentsVersion)

            andXmlBody(getBaseVisionResponse(response, serviceDefinition).replace('>', '|'))
        }
    }

    override fun responseWithExceptionWhenServiceUnavailable(): Mapping {
        return respondWith(HttpStatus.SC_SERVICE_UNAVAILABLE) {
            andXmlBody("").build()
        }
    }

    override fun responseErrorForbiddenService(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithExceptionWhenNotAvailable(): Mapping {

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                    serviceDefinition,
                    ErrorResponseCodeVision.APPOINTMENT_SLOT_NOT_FOUND)).build()
        }
    }

    override fun respondWithWithinAnHour(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun respondWithUnknownException(): Mapping {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    fun respondWithConflictException(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(VisionConstantsHelper.getBaseVisionFailedResponse(
                    serviceDefinition,
                    ErrorResponseCodeVision.APPOINTMENT_SLOT_NOT_BOOKED_TO_CURRENT_USER)).build()
        }
    }

    private fun retrieveMatchingReasonId(cancellationReason: String): String? {
        val reasons: List<AppointmentCancellationReason>? = Serenity
                .sessionVariableCalled<List<AppointmentCancellationReason>>(AppointmentCancellationReason::class)

        if (!cancellationReason.isBlank() && reasons != null) {
            for (i in 0 until reasons.size) {
                val reason = reasons[i]
                if (cancellationReason != reason.displayName) continue
                return reason.id
            }
        }
        return null
    }
}
