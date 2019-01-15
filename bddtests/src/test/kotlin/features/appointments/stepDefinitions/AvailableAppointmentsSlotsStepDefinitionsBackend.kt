package features.appointments.stepDefinitions

import cucumber.api.java.en.Given
import cucumber.api.java.en.When
import features.appointments.factories.AppointmentsSlotsFactory
import mocking.data.appointments.AppointmentsSlotsExample
import net.serenitybdd.core.Serenity
import worker.NhsoHttpException
import worker.WorkerClient
import worker.models.appointments.AppointmentSlotsResponse
import java.time.Duration
import javax.servlet.http.Cookie

private const val TIMEOUT_IN_SECONDS = 90L

class AvailableAppointmentsSlotsStepDefinitionsBackend  {

    @Given("^the system will time out when trying to retrieve (.*) appointment slots$")
    fun appointmentSlotsTimesOut(gpSystem: String) {
        val factory = AppointmentsSlotsFactory.getForSupplier(gpSystem)
        factory.generateExample {
            respondWithSuccess(AppointmentsSlotsExample.getGenericExample())
                    .delayedBy(Duration.ofSeconds(TIMEOUT_IN_SECONDS))
        }
    }

    @When("^the available appointment slots are retrieved without a cookie$")
    fun theAvailableAppointmentSlotsAreRetrievedWithoutACookie() {
        retrieveAppointmentSlots(null, null, includeCookie = false)
    }

    private fun retrieveAppointmentSlots(fromDate: String? = null,
                                         toDate: String? = null,
                                         includeCookie: Boolean = true) {
        var cookie: Cookie? = null
        if (includeCookie) {
            cookie = Serenity.sessionVariableCalled<Cookie>(Cookie::class)
        }

        try {
            val result = Serenity.sessionVariableCalled<WorkerClient>(WorkerClient::class)
                    .appointments.getAppointmentSlots(fromDate,
                            toDate,
                            cookie)
            Serenity.setSessionVariable(AppointmentSlotsResponse::class).to(result)
        } catch (httpException: NhsoHttpException) {
            Serenity.setSessionVariable("HttpException").to(httpException)
        }
    }
}
