package features.courses.stepDefinitions

import cucumber.api.java.en.And
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.courses.CoursesData
import mocking.MockDefaults.Companion.patient
import mocking.MockingClient
import mocking.emis.models.CourseRequestsGetResponse
import net.serenitybdd.core.Serenity
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.courses.CourseListResponse

open class coursesStepDefinitions {

    val EXPECTED_COURSE_DATA_SIZE = "CourseDataSize"
    val HTTP_EXCEPTION = "HttpException"

    val mockingClient = MockingClient.instance

    @And("^I have (\\d+) previous courses but only (\\d+) of type repeat of which (\\d+) can be requested")
    fun andIHaveXPreviousCoursesButOnlyXOfTypeRepeatOfWhichXCanBeRequested(numOfCourses: Int, numOfRepeats: Int, numCanBeRequested: Int)
    {
        val coursesData = CoursesData.getCourseData(numOfCourses, numOfRepeats, numCanBeRequested, mutableListOf())

        mockingClient.forEmis { coursesRequest(patient).respondWithSuccess(CourseRequestsGetResponse(coursesData)) }

        Serenity.setSessionVariable(EXPECTED_COURSE_DATA_SIZE).to(numCanBeRequested)
    }

    @When("I get the users courses with a valid cookie")
    fun whenIGetTheUsersCoursesWithAnValidCookie()
    {
        try {
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .getCoursesConnection(null)

            Serenity.setSessionVariable(CourseListResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("^I receive a list of (\\d+) repeating courses that can be requested$")
    fun thenIReceiveAListOfXRepeatingCoursesDateDescending(numOfCourses: Int) {
        val result = Serenity.sessionVariableCalled<CourseListResponse>(CourseListResponse::class)
        Assert.assertNotNull(result)
        Assert.assertEquals(numOfCourses, result.response.courses.count())
    }
}
