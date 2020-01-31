package features.im1Appointments.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import constants.Supplier
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import mocking.MockingClient
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.emis.appointments.AppointmentSlotsBuilderEmis
import mocking.emis.appointments.AppointmentSlotsMetaBuilderEmis
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.stubs.appointments.factories.AppointmentsSlotsFactory
import org.junit.Assert.assertEquals
import pages.ErrorPage
import pages.text
import java.time.Duration

private const val TIMEOUT_IN_SECONDS = 90L

private const val WAIT_FOR_TIMEOUT = 15000L

class AvailableAppointmentsSlotsTimeoutStepDefinitions {

    lateinit var errorPage: ErrorPage

    val mockingClient = MockingClient.instance
    private val appointmentSlotsExample = AppointmentsSlotsExample()

    @Given("^the (.*) doesn't respond in a timely fashion for available appointment slots$")
    fun theGpSystemDoesntRespondInATimelyFashionForAvailableAppointmentSlots(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        val factory = AppointmentsSlotsFactory.getForSupplier(supplier)
        val genericExample = appointmentSlotsExample.getGenericExample()
        factory.generateExample {
            withDelay(Duration.ofSeconds(TIMEOUT_IN_SECONDS))
                    .respondWithSuccess(genericExample)
        }
    }

    @Given("^the first request to the (.*) for available appointment slots times out but later requests succeed$")
    fun theFirstRequestToTheGpSystemForAvailableAppointmentSlotsTimesOutButLaterRequestsSucceed(gpSystem: String) {
        val supplier = Supplier.valueOf(gpSystem)
        timeoutFirstAppointmentSlotsRequestThenSucceed(supplier)
    }

    @Given("^the first request to EMIS for available appointment slots times out but later requests succeed$")
    fun theFirstRequestToEMISForAvailableAppointmentSlotsTimesOutButLaterRequestsSucceed() {
        timeoutFirstAppointmentSlotsRequestThenSucceed(Supplier.EMIS) { b -> b is AppointmentSlotsBuilderEmis }
    }

    @Given("^the first request to EMIS for available appointment slots metadata times out but later requests succeed$")
    fun theFirstRequestToEMISForAvailableAppointmentSlotsMetadataTimesOutButLaterRequestsSucceed() {
        timeoutFirstAppointmentSlotsRequestThenSucceed(Supplier.EMIS) { b -> b is AppointmentSlotsMetaBuilderEmis }
    }

    private fun timeoutFirstAppointmentSlotsRequestThenSucceed(
            gpSystem: Supplier,
            shouldTimeout : (IAppointmentSlotsBuilder) -> Boolean = { _ -> true }) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        val genericExample = appointmentSlotsExample.getGenericExample()

        // stub to generate timeout for 1st attempt
        factory.generateExample {
            if (shouldTimeout(this)) {
                withDelay(Duration.ofSeconds(TIMEOUT_IN_SECONDS))
                        .respondWithSuccess(genericExample)
                        .inScenario(timeoutScenario)
                        .whenScenarioStateIs(Scenario.STARTED)
                        .willSetStateTo(willSucceed)
            } else {
                respondWithSuccess(genericExample).inScenario(timeoutScenario)
            }
        }
        
        // stub to generate success on 2nd attempt
        factory.generateExample {
            if (shouldTimeout(this)) {
                respondWithSuccess(genericExample).inScenario(timeoutScenario).whenScenarioStateIs(willSucceed)
            } else {
                respondWithSuccess(genericExample).inScenario("None") // Request handled above
            }
        }
    }

    @Then("^I see appropriate information message for time-outs$")
    fun iSeeAppropriateInformationMessageAfterSecondsWhenItTimesOut() {
        Thread.sleep(WAIT_FOR_TIMEOUT)

        val expectedHeader = "There's been a problem loading this page"

        val expectedMessageText = "Try again now. If the problem continues and you need to book an appointment now, " +
                "contact your GP surgery directly. For urgent medical advice, call 111."

        assertEquals("expected Header text $expectedHeader but found ${errorPage.heading.text}",
                expectedHeader, errorPage.heading.text)

        assertEquals("expected error text $expectedMessageText but found ${errorPage.errorText1.text}",
                expectedMessageText, errorPage.errorText1.text)
    }

    @Then("^I see a timeout on the appointment booking page$")
    fun iSeeATimeOutOnTheAppointmentBookingPage() {
        errorPage.assertHeaderText("There's been a problem loading this page")
        errorPage.assertHasButton("Try again")
    }

    companion object {
        const val timeoutScenario = "timeout scenario"
        const val willSucceed = "to succeed"
    }
}
