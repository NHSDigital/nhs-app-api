package mocking.stubs.prescriptions

import mocking.MockingClient
import mocking.data.prescriptions.courses.EmisCoursesLoader
import mocking.emis.courses.EmisCoursesBuilder
import mocking.emis.models.CourseRequestsGetResponse
import mocking.stubs.InputResponse
import mocking.stubs.StubbedEnvironment.Companion.TIMEOUT_DELAY
import mocking.stubs.EmisStubsPatientFactory.Companion.goodPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.serviceNotEnabledPatientEMIS
import mocking.stubs.EmisStubsPatientFactory.Companion.timeoutPatientEMIS
import models.Patient
import models.prescriptions.MedicationCourse
import java.time.Duration

class ViewCoursesStubs(private val mockingClient: MockingClient) {
    fun generateEMISStubs(loadEMISCourses: MutableList<MedicationCourse>){
        val mapEMISViewCoursesRequestStubs =
                InputResponse<Patient, EmisCoursesBuilder>()
                        .addResponse(goodPatientEMIS) { builder
                            -> builder.respondWithSuccess(CourseRequestsGetResponse(loadEMISCourses)) }

                        .addResponse(serviceNotEnabledPatientEMIS) { builder
                            -> builder.respondWithPrescriptionsNotEnabled() }

                        .addResponse(timeoutPatientEMIS) { builder
                            -> builder.respondWithSuccess(CourseRequestsGetResponse(loadEMISCourses))
                                        .delayedBy(Duration.ofSeconds(TIMEOUT_DELAY)) }

        mapEMISViewCoursesRequestStubs.listResponse().forEach { scenario ->
            mockingClient.forEmis.mock { scenario.getResponse(prescriptions.coursesRequest(scenario.forMatcher)) }

        }
    }

    fun coursesLoaderEMIS(): MutableList<MedicationCourse> {
        val coursesLoader = EmisCoursesLoader
        coursesLoader.loadData(
                maxCourses = 1,
                numOfRepeats = 1,
                numCanBeRequested = 1,
                includeDosage = true,
                includeQuantity = true
        )

        return coursesLoader.data
    }
}
