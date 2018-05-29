package features.prescriptions.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import features.prescriptions.PrescriptionsData
import features.prescriptions.steps.PrescriptionsSteps
import mocking.MockDefaults.Companion.patient
import mocking.MockingClient
import mocking.emis.models.PrescriptionRequestsGetResponse
import net.serenitybdd.core.Serenity
import net.thucydides.core.annotations.Steps
import org.junit.Assert
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.prescriptions.PrescriptionListResponse
import java.time.OffsetDateTime
import java.time.ZonedDateTime

open class PrescriptionsStepDefinitions {
    @Steps
    lateinit var prescriptions: PrescriptionsSteps
    val mockingClient = MockingClient.instance
    val FROM_DATE = "FromDate"
    val TO_DATE = OffsetDateTime.now()
    val HTTP_EXCEPTION = "HttpException"
    val PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY: Long = 6


    @Then("^I see prescriptions page loaded$")
    fun iSeePrecriptionsPageLoaded() {
        prescriptions.isLoaded()
    }

    @Then("^I see a message indicating that I have no repeat prescriptions$")
    fun noRepeatPrescriptionsMessage() {
        prescriptions.assertNoRepeatPrescriptionsMessageShown()
    }

    @Given("From date is 6 months ago and I have 10 prescriptions in the last 6 months")
    fun givenFromDateIsSixMonthsAgoAndIHaveTenPrescriptionsInTheLastSixMonths()
    {
        val EXPECTED_DEFAULT_FROM_DATE = getDefaultPrescriptionsFromDate(TO_DATE)

        val prescriptionsData: PrescriptionRequestsGetResponse = PrescriptionsData.loadPrescriptionsData(10, 10, 10)

        mockingClient
                .forEmis {
                    prescriptionsRequest(patient, EXPECTED_DEFAULT_FROM_DATE, TO_DATE)
                            .respondWithSuccess(prescriptionsData)
                }

        Serenity.setSessionVariable(FROM_DATE).to(EXPECTED_DEFAULT_FROM_DATE)
    }

    @When("I get the users prescriptions with a valid cookie")
    fun whenIGetTheUsersPrescriptionsWithAValidCookie()
    {
        val formattedFromDate = Serenity.sessionVariableCalled<OffsetDateTime?>(FROM_DATE)

        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class).getPrescriptionsConnection(if(formattedFromDate != null) formattedFromDate.toString() else formattedFromDate, null)
            Serenity.setSessionVariable(PrescriptionListResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable(HTTP_EXCEPTION).to(httpException)
        }
    }

    @Then("I receive a list of 10 prescriptions")
    fun thenIReceiveAListOfTenPrescriptions() {
        val result = Serenity.sessionVariableCalled<PrescriptionListResponse>(PrescriptionListResponse::class)
        Assert.assertNotNull(result)
        Assert.assertEquals(10, result.response.prescriptions.count())
        val prescriptions = result.response.prescriptions

        // We had to use a string here and then parse the screen as kotlin did not like the date time format sent from the worker
        for(int in 0 until prescriptions.count()-2){
            Assert.assertTrue(ZonedDateTime.parse(prescriptions[int].orderDate) !!>= ZonedDateTime.parse(prescriptions[int+1].orderDate))
        }
    }

    fun getDefaultPrescriptionsFromDate(dateNow: OffsetDateTime): OffsetDateTime {
        return dateNow.minusMonths(PRESCRIPTIONS_DEFAULT_LAST_NUMBER_MONTHS_TO_DISPLAY)
    }

}