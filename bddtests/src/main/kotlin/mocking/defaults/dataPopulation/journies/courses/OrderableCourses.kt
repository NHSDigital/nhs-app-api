package mocking.defaults.dataPopulation.journies.courses

import mocking.MockingClient
import mocking.emis.models.CourseRequestsGetResponse
import models.Patient

private const val MAX_COURSES = 10
private const val NUMBER_OF_REPEATS = 10
private const val NUMBER_CAN_BE_REQUESTED = 10

class OrderableCourses(private val client: MockingClient) {

    fun createFor(patient: Patient) {

        client
                .forEmis {
                    prescriptions.coursesRequest(patient)
                            .respondWithSuccess(CourseRequestsGetResponse(
                                    CoursesData.getCourseData(
                                            MAX_COURSES,
                                            NUMBER_OF_REPEATS,
                                            NUMBER_CAN_BE_REQUESTED,
                                            mutableListOf(),
                                            true, true)))
                }
    }
}
