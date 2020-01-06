package mocking.stubs.prescriptions.factories

import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.TppPrescriptionLoader
import mocking.data.prescriptions.courses.TppCoursesLoader
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import mocking.tpp.models.ListRepeatMedicationReply
import mocking.tpp.models.RequestMedicationReply
import mockingFacade.prescriptions.PartialSuccessFacade
import models.Patient
import models.prescriptions.PrescriptionLoaderConfiguration
import org.apache.http.HttpStatus
import org.apache.http.HttpStatus.SC_FORBIDDEN
import utils.ProxySerenityHelpers
import utils.SerenityHelpers
import java.time.Duration
import java.time.OffsetDateTime

class PrescriptionsFactoryTpp: PrescriptionsFactory() {
    override val getCoursesLoader: ICoursesLoader<*> = TppCoursesLoader
    override val getPrescriptionsLoader: IPrescriptionLoader<*> = TppPrescriptionLoader

    override fun setupWiremockAndDataWithDelay(delay: Long?,
                                               prescriptionLoader: IPrescriptionLoader<*>,
                                               fromdate: OffsetDateTime,
                                               toDate: OffsetDateTime) {
        val currentPatient = SerenityHelpers.getPatient()
        val duration = if (delay != null) Duration.ofSeconds(delay) else null
        mockingClient
                .forTpp {
                    prescriptions.listRepeatMedication(currentPatient)
                            .respondWithSuccess(prescriptionLoader.data as ListRepeatMedicationReply)
                            .delayedBy(duration)
                }
    }

    override fun setupWireMockAndDataSetup(scenarioTitle: String,
                                           initialScenarioState: String,
                                           statusSubmitted: String,
                                           initialHistoricPrescriptionsCount: Int,
                                           amount: Int): String {
        val courses = getCoursesLoader.data as ListRepeatMedicationReply

        val patient = ProxySerenityHelpers.getPatientOrProxy()
        mockingClient.forTpp {
            prescriptions.prescriptionSubmission(patient, courses.Medication.map { it.drugId })
                    .respondWithSuccess(RequestMedicationReply(patient.patientId, patient.onlineUserId))
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(initialScenarioState)
        }
        val numberOfPrescriptionsAfterSubmit = amount + initialHistoricPrescriptionsCount

        val prescriptionLoaderConfig = PrescriptionLoaderConfiguration(
            noPrescriptions = numberOfPrescriptionsAfterSubmit,
            noCourses = numberOfPrescriptionsAfterSubmit,
            noRepeats = numberOfPrescriptionsAfterSubmit
        )

        getPrescriptionsLoader.loadData(prescriptionLoaderConfig)

        val newPrescriptions = getPrescriptionsLoader.data as ListRepeatMedicationReply
        mockingClient.forTpp {
            prescriptions.listRepeatMedication(patient)
                    .respondWithSuccess(newPrescriptions)
        }

        return initialScenarioState
    }

    override fun setupWireMockAndCreateDataGpSpecific() {

        mockingClient.forTpp {
            prescriptions.listRepeatMedication(ProxySerenityHelpers.getPatientOrProxy())
                    .respondWithSuccess(getCoursesLoader.data as ListRepeatMedicationReply)
        }
    }

    override fun gpSessionHasExpired() {
        mockingClient.forTpp {
            prescriptions.listRepeatMedication(ProxySerenityHelpers.getPatientOrProxy())
                    .respondWithTppNotAuthorised("")
        }
    }

    override fun disableAtGPLevel() {
        mockingClient
                .forTpp {
                    prescriptions.listRepeatMedication(ProxySerenityHelpers.getPatientOrProxy())
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

    override fun orderPrescriptionReturnsConflictResponse() {
        throw UnsupportedOperationException()
    }

    override fun prescriptionsOrderEndpointPartiallySuccessful(partialSuccess: PartialSuccessFacade) {
        throw UnsupportedOperationException()
    }

    override fun disableForProxy(callingPatient: Patient, actingOnBehalfOf: Patient) {
        throw UnsupportedOperationException()
    }
}
