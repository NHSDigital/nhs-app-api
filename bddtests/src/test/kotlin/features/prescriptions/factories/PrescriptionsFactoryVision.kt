package features.prescriptions.factories

import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.VisionPrescriptionLoader
import mocking.data.prescriptions.courses.VisionCoursesLoader
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import mocking.defaults.VisionMockDefaults
import mocking.stubs.prescriptions.ViewPrescriptionsStubs
import mocking.vision.models.EligibleRepeats
import mocking.vision.models.NewPrescriptionRepeat
import mocking.vision.models.OrderNewPrescriptionRequest
import mocking.vision.models.OrderNewPrescriptionResponse
import mocking.vision.models.PrescriptionHistory
import mocking.vision.models.VisionUserSession
import models.Patient
import org.apache.http.HttpStatus

class PrescriptionsFactoryVision: PrescriptionsFactory("VISION") {

    override val getCoursesLoader: ICoursesLoader<*> = VisionCoursesLoader
    override val getPrescriptionsLoader: IPrescriptionLoader<*> = VisionPrescriptionLoader

    override fun setupWireMockAndDataSetup(scenarioTitle: String,
                                           initialScenarioState: String,
                                           statusSubmitted: String,
                                           initialHistoricPrescriptionsCount: Int,
                                           amount: Int): String {

        val courses = getCoursesLoader.data as EligibleRepeats

        val request = OrderNewPrescriptionRequest(
                patient.patientId,
                courses.repeat.map { NewPrescriptionRepeat(it.getRepeatCourseId()!!) },
                "")

        mockingClient.forVision {
            prescriptions.orderNewPrescriptionRequest(VisionMockDefaults.visionUserSession, request)
                    .respondWithSuccess(OrderNewPrescriptionResponse.Ok)
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(initialScenarioState)
                    .willSetStateTo(statusSubmitted)
        }
        val numberOfPrescriptionsAfterSubmit = amount + initialHistoricPrescriptionsCount
        getPrescriptionsLoader.loadData(numberOfPrescriptionsAfterSubmit,
                numberOfPrescriptionsAfterSubmit,
                numberOfPrescriptionsAfterSubmit)
        val newPrescriptions = getPrescriptionsLoader.data as PrescriptionHistory
        mockingClient.forVision {
            prescriptions.getPrescriptionHistoryRequest(VisionUserSession.fromPatient(patient))
                    .respondWithSuccess(newPrescriptions)
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(statusSubmitted)
        }
        return initialScenarioState
    }

    override fun setupWireMockAndCreateDataGpSpecific() {
        val userSession = VisionUserSession.fromPatient(patient)

        mockingClient.forVision {
            prescriptions.getEligibleRepeatsRequest(userSession)
                    .respondWithSuccess(getCoursesLoader.data as EligibleRepeats)
        }
    }

    override fun gpSessionHasExpired() {
        throw NotImplementedError()
    }

    override fun disableAtGPLevel() {
        mockingClient
            .forVision {
                authentication.getConfigurationRequest(
                        VisionMockDefaults.visionUserSessionPrescriptionDisabled)
                        .respondWithSuccess(VisionMockDefaults
                                .visionConfigurationResponsePrescriptionsDisabled)
            }}

    override fun prescriptionsEndpointTimeout(patient: Patient) {
        mockingClient
                .forVision {
                    prescriptions.getPrescriptionHistoryRequest(VisionUserSession.fromPatient(patient))
                            .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000)
                }
    }

    override fun prescriptionsEndpointThrowServerError(patient: Patient) {
        mockingClient
                .forVision {
                    prescriptions.getPrescriptionHistoryRequest(VisionUserSession.fromPatient(patient))
                            .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {})
                }
    }

    override fun coursesEndpointTimeout(patient: Patient) {
        mockingClient
                .forVision {
                    prescriptions.getEligibleRepeatsRequest(VisionUserSession.fromPatient(patient))
                            .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000)
                }
    }

    override fun coursesEndpointThrowingServerError(patient: Patient) {
        mockingClient
                .forVision {
                    prescriptions.getEligibleRepeatsRequest(VisionUserSession.fromPatient(patient))
                            .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {})
                }
    }

    override fun generateSpineStubs() {
        ViewPrescriptionsStubs(mockingClient).generateSpineStubs()
    }
}
