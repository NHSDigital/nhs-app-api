package features.prescriptions.factories

import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.TppPrescriptionLoader
import mocking.data.prescriptions.courses.TppCoursesLoader
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import mocking.stubs.prescriptions.ViewPrescriptionsStubs
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.tpp.models.RequestMedicationReply
import models.Patient
import org.apache.http.HttpStatus
import org.apache.http.HttpStatus.SC_FORBIDDEN

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

    override fun gpSessionHasExpired() {
        mockingClient.forTpp {
            prescriptions.listRepeatMedication(patient)
                    .respondWithTppNotAuthorised("")
        }
    }

    override fun disableAtGPLevel() {
        mockingClient
                .forTpp {
                    prescriptions.listRepeatMedication(patient)
                            .respondWith(SC_FORBIDDEN, 0, resolve = {})
                }
    }

    override fun prescriptionsEndpointTimeout(patient: Patient) {
        mockingClient
                .forTpp {
                    prescriptions.listRepeatMedication(patient)
                            .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000)
                }
    }

    override fun prescriptionsEndpointThrowServerError(patient: Patient) {
        mockingClient
                .forTpp {
                    prescriptions.listRepeatMedication(patient)
                            .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {})
                }
    }

    override fun coursesEndpointTimeout(patient: Patient) {
        Thread.sleep(TIME_TO_SLEEP_IN_MILLIS)
        mockingClient
                .forTpp {
                    prescriptions.listRepeatMedication(patient)
                            .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000)
                }
    }

    override fun coursesEndpointThrowingServerError(patient: Patient) {
        Thread.sleep(TIME_TO_SLEEP_IN_MILLIS)
        mockingClient
                .forTpp {
                    prescriptions.listRepeatMedication(patient)
                            .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {})
                }
    }

    override fun generateSpineStubs() {
        ViewPrescriptionsStubs(mockingClient).generateSpineStubs()
    }
}
