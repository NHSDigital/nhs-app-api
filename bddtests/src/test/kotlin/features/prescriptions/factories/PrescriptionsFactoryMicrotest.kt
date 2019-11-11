package features.prescriptions.factories

import com.github.tomakehurst.wiremock.stubbing.Scenario
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.MicrotestPrescriptionLoader
import mocking.data.prescriptions.courses.MicrotestCoursesLoader
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import mocking.microtest.prescriptions.Course
import mocking.microtest.prescriptions.CoursesGetResponse
import mocking.microtest.prescriptions.PrescriptionHistoryGetResponse
import mockingFacade.prescriptions.PartialSuccessFacade
import models.Patient
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus
import utils.SerenityHelpers
import java.time.Duration
import java.time.OffsetDateTime

class PrescriptionsFactoryMicrotest: PrescriptionsFactory("MICROTEST") {

    override val getCoursesLoader: ICoursesLoader<*> = MicrotestCoursesLoader
    override val getPrescriptionsLoader: IPrescriptionLoader<*> = MicrotestPrescriptionLoader

    override fun setupWireMockAndCreateDataGpSpecific() {
        val response = coursesData()
        mockingClient.forMicrotest {
            prescriptions.getCoursesRequest(patient).respondWithSuccess(CoursesGetResponse(response))
        }
    }

    @Suppress("UNCHECKED_CAST",
            "Cast cannot be checked as a generic type is used, " +
                    "see https://kotlinlang.org/docs/reference/typecasts.html")
    private fun coursesData(): List<Course> {
        return getCoursesLoader.data as List<Course>
    }

    override fun setupWiremockAndDataWithDelay(delay: Long?,
                                               prescriptionLoader: IPrescriptionLoader<*>,
                                               fromdate: OffsetDateTime,
                                               toDate: OffsetDateTime) {

        val currentPatient = SerenityHelpers.getPatient()
        val duration = if (delay != null) Duration.ofSeconds(delay) else null
        mockingClient
                .forMicrotest {
                    prescriptions.getPrescriptionHistoryRequest(currentPatient, fromdate)
                            .respondWithSuccess(prescriptionLoader.data as PrescriptionHistoryGetResponse)
                            .delayedBy(duration)
                }
    }

    override fun setupWireMockAndDataSetup(scenarioTitle: String,
                                           initialScenarioState: String,
                                           statusSubmitted: String,
                                           initialHistoricPrescriptionsCount: Int,
                                           amount: Int): String {

        var currentScenarioState = initialScenarioState
        val courses = coursesData()

        mockingClient.forMicrotest {
            prescriptions.getCoursesRequest(patient)
                    .respondWithSuccess(CoursesGetResponse(courses))
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
        }

        mockingClient.forMicrotest {
            prescriptions.repeatPrescriptionSubmissionRequest(patient)
                    .respondWithCreated()
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
                    .willSetStateTo(statusSubmitted)
        }

        currentScenarioState = statusSubmitted

        val microtestPrescriptionMap = Serenity.sessionVariableCalled<MutableMap<String,
                PrescriptionHistoryGetResponse>>("MicrotestPrescriptionsMap")

        microtestPrescriptionMap[currentScenarioState] = MicrotestPrescriptionLoader.orderCourses(
                orderedCourses = courses.toMutableList(),
                oldPrescriptionHistory = microtestPrescriptionMap[Scenario.STARTED]!!)

        mockingClient.forMicrotest {
            prescriptions.getPrescriptionHistoryRequest(patient)
                    .respondWithSuccess(microtestPrescriptionMap[currentScenarioState]!!)
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
        }

        return currentScenarioState
    }

    override fun prescriptionsEndpointTimeout(patient: Patient) {
        mockingClient
                .forMicrotest {
                    prescriptions.getPrescriptionHistoryRequest(patient)
                            .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000)
                }
    }

    override fun prescriptionsEndpointThrowServerError(patient: Patient) {
        mockingClient
                .forMicrotest {
                    prescriptions.getPrescriptionHistoryRequest(patient)
                            .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {})
                }
    }

    override fun coursesEndpointTimeout(patient: Patient) {
        mockingClient.forMicrotest {
            prescriptions.getCoursesRequest(patient)
                    .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000)
        }
    }

    override fun coursesEndpointThrowingServerError(patient: Patient) {
        mockingClient.forMicrotest {
            prescriptions.getCoursesRequest(patient)
                    .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {})
        }
    }

    override fun gpSessionHasExpired() {
        throw NotImplementedError()
    }

    override fun disableAtGPLevel() {
        mockingClient
                .forMicrotest {
                    prescriptions.getPrescriptionHistoryRequest(patient)
                            .respondWithForbiddenError()
                }
    }

    override fun orderPrescriptionReturnsConflictResponse() {
        mockingClient.forMicrotest {
            prescriptions.repeatPrescriptionSubmissionRequest(patient)
                    .respondWithConflictError()
        }
    }

    override fun prescriptionsOrderEndpointPartiallySuccessful(partialSuccess: PartialSuccessFacade) {
        mockingClient.forMicrotest {
            prescriptions.repeatPrescriptionSubmissionRequest(patient)
                    .respondWithPartiallySuccessful(partialSuccess)
                    .whenScenarioStateIs(Scenario.STARTED)
        }
    }

    override fun disableForProxy(callingPatient: Patient, actingOnBehalfOf: Patient) {
        throw UnsupportedOperationException()
    }
}
