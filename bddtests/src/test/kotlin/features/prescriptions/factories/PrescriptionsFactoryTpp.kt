package features.prescriptions.factories

import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.TppPrescriptionLoader
import mocking.data.prescriptions.courses.TppCoursesLoader
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.tpp.models.RequestMedicationReply

class PrescriptionsFactoryTpp: PrescriptionsFactory("TPP") {

    override val getCoursesLoader: ICoursesLoader<*> = TppCoursesLoader
    override val getPrescriptionsLoader: IPrescriptionLoader<*> = TppPrescriptionLoader

    override fun setupWireMockAndDataSetup(scenarioTitle: String,
                                           initialScenarioState: String,
                                           statusSubmitted: String,
                                           initialHistoricPrescriptionsCount: Int,
                                           amount: Int): String {
        val courses = getCoursesLoader.data as ListRepeatMedicationReply

        mockingClient.forTpp {
            prescriptions.prescriptionSubmission(patient, courses.Medication.map { it.drugId })
                    .respondWithSuccess(RequestMedicationReply(patient.patientId, patient.onlineUserId))
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(initialScenarioState)
        }
        val numberOfPrescriptionsAfterSubmit = amount + initialHistoricPrescriptionsCount
        getPrescriptionsLoader.loadData(numberOfPrescriptionsAfterSubmit,
                numberOfPrescriptionsAfterSubmit,
                numberOfPrescriptionsAfterSubmit)
        val newPrescriptions = getPrescriptionsLoader.data as ListRepeatMedicationReply
        mockingClient.forTpp {
            prescriptions.listRepeatMedication(patient)
                    .respondWithSuccess(newPrescriptions)
        }

        return initialScenarioState
    }

    override fun setupWireMockAndCreateDataGpSpecific() {

        mockingClient.forTpp {
            prescriptions.listRepeatMedication(patient)
                    .respondWithSuccess(getCoursesLoader.data as ListRepeatMedicationReply)
        }
    }

    override fun disableAtGPLevel() {
        mockingClient
                .forTpp {
                    prescriptions.listRepeatMedication(patient)
                            .respondWith(403, 0, resolve = {})
                }
    }
}
