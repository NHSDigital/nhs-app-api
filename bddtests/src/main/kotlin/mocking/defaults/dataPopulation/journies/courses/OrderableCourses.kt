package mocking.defaults.dataPopulation.journies.courses

import mocking.MockingClient
import mocking.emis.models.CourseRequestsGetResponse
import models.Patient

class OrderableCourses(private val client: MockingClient) {

    fun createFor(patient: Patient) {

        client
                .forEmis {
                    coursesRequest(patient)
                            .respondWithSuccess(CourseRequestsGetResponse(
                                    CoursesData.getCourseData(
                                            10,
                                            10,
                                            10,
                                            mutableListOf(),
                                            true, true)))
                }
    }
}