package mocking.defaults.dataPopulation.journies.courses

import mocking.MockingClient
import mocking.emis.models.CourseRequestsGetResponse
import models.Patient

private const val MAX_COURSES = 10
private const val NUMBER_OF_REPEATS = 10
private const val NUMBER_CAN_BE_REQUESTED = 10

class OrderableCourses(private val client: MockingClient) {

    fun createFor(patient: Patient) {

        val courses =
                CoursesDataBuilder()
                        .maxCourses(MAX_COURSES)
                        .numOfRepeats(NUMBER_OF_REPEATS)
                        .numCanBeRequested(NUMBER_CAN_BE_REQUESTED)
                        .medicationCourses(mutableListOf())
                        .includeDosage(true)
                        .includeQuantity(true)
                        .build()
        client
                .forEmis {
                    prescriptions.coursesRequest(patient)
                            .respondWithSuccess(CourseRequestsGetResponse(courses))
                }
    }
}

