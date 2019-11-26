package features.courses.stepDefinitions

import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.linkedProfiles.LinkedProfilesSerenityHelpers
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.SerenityHelpers
import utils.getOrFail
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.courses.CoursesListResponse

class CoursesStepDefinitionsBackend {

    @When("I get the users courses with a valid cookie")
    fun whenIGetTheUsersCoursesWithAnValidCookie() {
        try {
            val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
            val result = Serenity
                    .sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .prescriptions.getCoursesConnection(patientId)

            Serenity.setSessionVariable(CoursesListResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            SerenityHelpers.setHttpException(httpException)
        }
    }

    @Then("^I receive a list of (\\d+) repeating prescriptions that can be requested$")
    fun thenIReceiveAListOfXRepeatingPrescriptionsDateDescending(expectedNumberOfRepeatingPrescriptions: Int) {
        val result = Serenity.sessionVariableCalled<CoursesListResponse>(CoursesListResponse::class)
        Assert.assertNotNull("No courses in session", result)

        val actualNumberOfRepeatPrescriptions = result.courses.count()

        Assert.assertEquals(
                "Unexpected number of repeat prescriptions. Expected: "
                        + expectedNumberOfRepeatingPrescriptions
                        + ". Actual: "
                        + actualNumberOfRepeatPrescriptions,
                expectedNumberOfRepeatingPrescriptions,
                result.courses.count())
    }
}
