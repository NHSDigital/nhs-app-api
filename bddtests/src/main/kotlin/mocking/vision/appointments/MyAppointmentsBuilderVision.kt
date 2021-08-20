package mocking.vision.appointments

import mocking.gpServiceBuilderInterfaces.appointments.IMyAppointmentsBuilder
import mocking.models.Mapping
import mocking.vision.VisionConstants
import mocking.vision.VisionConstants.getVisionExistingAppointmentsResponse
import mocking.vision.VisionDirectServicesErrorResponses
import mocking.vision.VisionDirectServicesMappingBuilder
import mocking.vision.appointments.helpers.MyAppointmentsHelper.Companion.extractResponseFromFacade
import mocking.vision.helpers.VisionConstantsHelper
import mocking.vision.models.ServiceDefinition
import mocking.vision.models.VisionUserSession
import mocking.vision.models.appointments.BookedAppointmentsResponse
import mockingFacade.appointments.MyAppointmentsFacade
import models.Patient
import org.apache.http.HttpStatus
import java.io.StringWriter
import javax.xml.bind.JAXBContext
import javax.xml.bind.Marshaller


class MyAppointmentsBuilderVision(val patient: Patient) : VisionDirectServicesMappingBuilder(
        orgId = patient.odsCode, path = "existingAppointments"), IMyAppointmentsBuilder {

    private val serviceDefinition = ServiceDefinition(
            VisionConstants.existingAppointmentsName,
            VisionConstants.existingAppointmentsVersion
    )

    init {

        val userSession = VisionUserSession(
                patient.rosuAccountId,
                patient.apiKey,
                patient.odsCode,
                patient.patientId
        )
        requestBuilder
                .andBody("<vision:rosuAccountId>${userSession.rosuAccountId}</vision:rosuAccountId>", "contains")
                .andBody("<vision:apiKey>${userSession.apiKey}</vision:apiKey>", "contains")
                .andBody("<vision:provider>${userSession.provider}</vision:provider>", "contains")
                .andBody("<vision:accountId>${userSession.accountId}</vision:accountId>", "contains")
                .andBody("<vision:patientId>${userSession.patientId}</vision:patientId>", "contains")
    }

    override fun respondWithGPErrorWhenNotEnabled(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(VisionDirectServicesErrorResponses.getServiceNotEnabled(serviceDefinition)).build()
        }
    }

    override fun respondWithUnknownException(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(VisionDirectServicesErrorResponses.getUnknownVisionError(serviceDefinition)).build()
        }
    }

    override fun respondWithUnauthorised(): Mapping {
        return respondWith(HttpStatus.SC_BAD_REQUEST) {
            andXmlBody(VisionDirectServicesErrorResponses.getInvalidUserCredentialsError(serviceDefinition)).build()
        }
    }

    override fun respondWithCorrupted(): Mapping {
        return respondWith(HttpStatus.SC_OK) {
            val corruptedResponse = VisionConstantsHelper
                    .getBaseVisionDirectServicesResponse("", serviceDefinition)
                    .replace(">", "|")
                    .replace("}", "|")

            andBody(corruptedResponse, contentType = "text/xml")
        }
    }

    override fun respondWithSuccess(body: String): Mapping {
        return respondWithBody(body)
    }
    override fun respondWithSuccess(facade: MyAppointmentsFacade): Mapping {
        val bookedAppointmentsResponse = extractResponseFromFacade(facade.myAppointments!!)

        val jaxbContext = JAXBContext.newInstance(BookedAppointmentsResponse::class.java)
        val marshaller = jaxbContext.createMarshaller()
        marshaller.setProperty(Marshaller.JAXB_FORMATTED_OUTPUT, true)

        val stringWriter = StringWriter()
        stringWriter.use {
            marshaller.marshal(bookedAppointmentsResponse, stringWriter)
        }

        return respondWith(HttpStatus.SC_OK) {
            andXmlBody(getVisionExistingAppointmentsResponse(stringWriter.toString(), serviceDefinition)).build()
        }
    }
}
