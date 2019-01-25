package features.prescriptions.factories

import com.github.tomakehurst.wiremock.stubbing.Scenario
import mocking.data.prescriptions.EmisPrescriptionLoader
import mocking.data.prescriptions.IPrescriptionLoader
import mocking.data.prescriptions.courses.EmisCoursesLoader
import mocking.emis.models.CourseRequestsGetResponse
import mocking.emis.models.PrescriptionRequestsGetResponse
import mocking.gpServiceBuilderInterfaces.courses.ICoursesLoader
import models.Patient
import models.prescriptions.MedicationCourse
import net.serenitybdd.core.Serenity
import org.apache.http.HttpStatus

class PrescriptionsFactoryEmis: PrescriptionsFactory("EMIS") {

    override val getCoursesLoader: ICoursesLoader<*> = EmisCoursesLoader
    override val getPrescriptionsLoader: IPrescriptionLoader<*> = EmisPrescriptionLoader

    override fun setupWireMockAndCreateDataGpSpecific() {
        val response = CourseRequestsGetResponse(coursesData())
        mockingClient.forEmis {
            prescriptions.coursesRequest(patient).respondWithSuccess(response)
        }
    }

    @Suppress("UNCHECKED_CAST",
            "Cast cannot be checked as a generic type is used, " +
                    "see https://kotlinlang.org/docs/reference/typecasts.html")
    private fun coursesData(): List<MedicationCourse> {
        return getCoursesLoader.data as List<MedicationCourse>
    }

    override fun setupWireMockAndDataSetup(scenarioTitle: String,
                                           initialScenarioState: String,
                                           statusSubmitted: String,
                                           initialHistoricPrescriptionsCount: Int,
                                           amount: Int): String {

        var currentScenarioState = initialScenarioState
        val courses = coursesData()

        mockingClient.forEmis {
            prescriptions.coursesRequest(patient)
                    .respondWithSuccess(CourseRequestsGetResponse(courses))
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
        }

        mockingClient.forEmis {
            prescriptions.repeatPrescriptionSubmissionRequest(patient)
                    .respondWithCreated()
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
                    .willSetStateTo(statusSubmitted)
        }

        currentScenarioState = statusSubmitted

        val emisPrescriptionMap = Serenity.sessionVariableCalled<MutableMap<String,
                PrescriptionRequestsGetResponse>>("EmisPrescriptionsMap")
        emisPrescriptionMap[currentScenarioState] = EmisPrescriptionLoader.orderCourses(
                orderedCourses = courses.toMutableList(),
                oldPrescriptions = emisPrescriptionMap[Scenario.STARTED]!!)

        mockingClient.forEmis {
            prescriptions.prescriptionsRequest(patient)
                    .respondWithSuccess(emisPrescriptionMap[currentScenarioState]!!)
                    .inScenario(scenarioTitle)
                    .whenScenarioStateIs(currentScenarioState)
        }

        return currentScenarioState
    }

    override fun gpSessionHasExpired() {
        mockingClient.forEmis {
            prescriptions.prescriptionsRequest(patient)
                    .respondWithEmisNotAuthorised()
        }
    }

    override fun disableAtGPLevel() {
        mockingClient
                .forEmis {
                    prescriptions.prescriptionsRequest(patient)
                            .respondWithPrescriptionsNotEnabled()
                }
    }

    override fun prescriptionsEndpointTimeout(patient: Patient) {
        mockingClient
                .forEmis {
                    prescriptions.prescriptionsRequest(patient)
                            .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000)
                }
    }

    override fun prescriptionsEndpointThrowServerError(patient: Patient) {
        mockingClient
                .forEmis {
                    prescriptions.prescriptionsRequest(patient)
                            .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {})
                }
    }

    override fun coursesEndpointTimeout(patient: Patient) {
        mockingClient.forEmis {
            prescriptions.coursesRequest(patient)
                    .respondWith(HttpStatus.SC_GATEWAY_TIMEOUT, resolve = {}, milliSecondDelay = 15000)
        }
    }

    override fun coursesEndpointThrowingServerError(patient: Patient) {
        mockingClient.forEmis {
            prescriptions.coursesRequest(patient)
                    .respondWith(HttpStatus.SC_INTERNAL_SERVER_ERROR, resolve = {})
        }
    }
}
