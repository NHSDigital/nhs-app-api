package features.prescriptions.stepDefinitions

import constants.Supplier
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import net.serenitybdd.core.Serenity
import org.junit.Assert
import utils.LinkedProfilesSerenityHelpers
import utils.getOrFail
import utils.getOrNull
import utils.set
import worker.WorkerClient
import worker.models.prescriptions.PrescriptionsListResponse
import java.time.OffsetDateTime
import java.time.ZonedDateTime

class PrescriptionsStepDefinitionsBackend {

    private val fromDateKey = "FromDate"

    @When("I request the users prescriptions with a valid cookie")
    fun whenIRequestTheUsersPrescriptionsWithAValidCookie() {
        val formattedFromDate = Serenity.sessionVariableCalled<OffsetDateTime?>(fromDateKey)
        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrNull<String>()
        val sessionVariable = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
        val response = sessionVariable.prescriptions.getPrescriptionsConnection(patientId,
                if (formattedFromDate != null) formattedFromDate.toString() else formattedFromDate)
        PrescriptionsSerenityHelpers.PRESCRIPTIONS_LIST_RESPONSE.set(response)
    }

    @When("^I request prescriptions for the last 6 months$")
    fun iRequestPrescriptionsForTheLastSixMonths() {
        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrNull<String>()
        val fromDate = PrescriptionsSerenityHelpers.FROM_DATE.getOrNull<String>()
        val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .prescriptions.getPrescriptionsConnection(patientId, fromDate)
        PrescriptionsSerenityHelpers.PRESCRIPTIONS_LIST_RESPONSE.set(response)
    }

    @When("^I request prescriptions for the last 6 months with an invalid cookie$")
    fun iRequestPrescriptionsForTheLastSixMonthsWithAnInvalidCookie() {
        val patientId = LinkedProfilesSerenityHelpers.MAIN_PATIENT_ID.getOrFail<String>()
        val fromDate = PrescriptionsSerenityHelpers.FROM_DATE.getOrNull<String>()
        val response = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                .prescriptions.getPrescriptionsConnection(patientId, fromDate, WorkerClient.getHttpContext(true))
        PrescriptionsSerenityHelpers.PRESCRIPTIONS_LIST_RESPONSE.set(response)
    }

    @Then("I receive a list of (\\d+) prescriptions")
    fun thenIReceiveAListOfXPrescriptions(count: Int) {

        val prescriptionsListResponse = PrescriptionsSerenityHelpers.PRESCRIPTIONS_LIST_RESPONSE
                .getOrFail<PrescriptionsListResponse>()
        Assert.assertNotNull(prescriptionsListResponse)
        val currentProvider = PrescriptionsSerenityHelpers.PROVIDER.getOrNull<Supplier>()

        when (currentProvider) {
            Supplier.EMIS -> {
                Assert.assertEquals(count, prescriptionsListResponse.prescriptions.count())
                val prescriptions = prescriptionsListResponse.prescriptions

                // We had to use a string here and then parse the screen as
                // kotlin did not like the date time format sent from the worker
                for (int in 0 until prescriptions.count() - 2) {
                    Assert.assertTrue(ZonedDateTime.parse(prescriptions[int].orderDate)!! >=
                            ZonedDateTime.parse(prescriptions[int + 1].orderDate))
                }
            }
            Supplier.TPP,
            Supplier.VISION,
            Supplier.MICROTEST -> {
                Assert.assertEquals(count, prescriptionsListResponse.courses.count())
            }
            else -> {
                throw NotImplementedError("Invalid GP System")
            }
        }
    }

    @Then("^I get a response with a list of prescriptions for the last 6 months$")
    fun iGetAResponseWithAListOfPrescriptionForTheLastSixMonths() {
        val prescriptionsListResponse = PrescriptionsSerenityHelpers.PRESCRIPTIONS_LIST_RESPONSE
                .getOrFail<PrescriptionsListResponse>()
        Assert.assertNotNull(prescriptionsListResponse)
        Assert.assertTrue(prescriptionsListResponse.prescriptions.isNotEmpty())
        Assert.assertTrue(prescriptionsListResponse.courses.isNotEmpty())
    }
}