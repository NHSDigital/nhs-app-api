package features.prescriptions.factories

import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.VisionPrescriptionLoader
import mocking.data.prescriptions.courses.VisionCoursesLoader
import mocking.gpServiceBuilderInterfaces.Courses.ICoursesLoader
import mocking.vision.VisionMockDefaults
import mocking.vision.models.EligibleRepeats
import mocking.vision.models.NewPrescriptionRepeat
import mocking.vision.models.OrderNewPrescriptionRequest
import mocking.vision.models.OrderNewPrescriptionResponse
import mocking.vision.models.PrescriptionHistory
import mocking.vision.models.VisionUserSession

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
            orderNewPrescriptionRequest(VisionMockDefaults.visionUserSession, request)
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
            getPrescriptionHistoryRequest(VisionUserSession.fromPatient(patient))
                    .respondWithSuccess(newPrescriptions)
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(statusSubmitted)
        }
        return initialScenarioState
    }

    override fun setupWireMockAndCreateDataGpSpecific() {
        val userSession = VisionUserSession.fromPatient(patient)

        mockingClient.forVision {
            getEligibleRepeatsRequest(userSession)
                    .respondWithSuccess(getCoursesLoader.data as EligibleRepeats)
        }
    }

    override fun disableAtGPLevel() {}
}
