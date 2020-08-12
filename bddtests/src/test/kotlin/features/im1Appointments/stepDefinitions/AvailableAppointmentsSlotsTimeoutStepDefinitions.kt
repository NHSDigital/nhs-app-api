package features.im1Appointments.stepDefinitions

import com.github.tomakehurst.wiremock.stubbing.Scenario
import constants.Supplier
import io.cucumber.java.en.Given
import mocking.data.appointments.AppointmentsSlotsExample
import mocking.emis.appointments.AppointmentSlotsBuilderEmis
import mocking.emis.appointments.AppointmentSlotsMetaBuilderEmis
import mocking.gpServiceBuilderInterfaces.appointments.IAppointmentSlotsBuilder
import mocking.stubs.appointments.factories.AppointmentsSlotsFactory
import java.time.Duration

private const val TIMEOUT_IN_SECONDS = 90L

class AvailableAppointmentsSlotsTimeoutStepDefinitions {

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

    companion object {
        const val timeoutScenario = "timeout scenario"
        const val willSucceed = "to succeed"
    }
}
