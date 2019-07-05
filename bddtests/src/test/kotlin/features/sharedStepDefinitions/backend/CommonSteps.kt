package features.sharedStepDefinitions.backend

import config.Config
import cucumber.api.java.Before
import cucumber.api.java.After
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import mocking.MockingClient
import mocking.defaults.VisionMockDefaults
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import mocking.tpp.models.Authenticate
import models.Patient
import net.serenitybdd.core.Serenity
import net.serenitybdd.core.Serenity.getCurrentSession
import net.serenitybdd.core.Serenity.setSessionVariable
import net.serenitybdd.core.Serenity.getWebdriverManager
import net.serenitybdd.core.Serenity.sessionVariableCalled
import org.junit.Assert
import utils.SerenityHelpers
import utils.contains
import worker.WorkerClient
import java.util.concurrent.TimeUnit
import java.util.logging.Level

private const val ADDITIONAL_TIME_TO_DELAY = 10
val CONSOLE_LOG_STRINGS_TO_IGNORE =
        arrayOf("favicon.ico"
                ,"redirectToCitizenId"
                ,"Failed to load resource"
                ,"Request failed with status code 50")

class CommonSteps : AbstractSteps() {
    companion object {
        val GP_SYSTEM: String = "GpSupplier"
    }

    @Before
    fun beforeEachScenario() {
        getCurrentSession().clear()

        mockingClient = MockingClient.instance
        workerClient = WorkerClient()
        mockingClient.clearWiremock()

        setSessionVariable(MockingClient::class).to(mockingClient)
        setSessionVariable(WorkerClient::class).to(workerClient)
    }

    @After
    fun afterEachScenario() {

        val driver = getWebdriverManager().currentDriver

        if(driver!=null) {
            val logs = driver.manage().logs().get("browser")
                    .filter(Level.SEVERE)
                    .filterNot { it.message.contains(CONSOLE_LOG_STRINGS_TO_IGNORE) }

            Assert.assertTrue("There should not be any console logs but found: \r\n $logs",
                    logs.isEmpty())
        }
    }

    @Given("^(EMIS|TPP|VISION) is not available$")
    fun givenXIsNotAvailable(gpSystem: String) {

        val patient = Patient.getDefault(gpSystem)
        setSessionVariable("ConnectionToken").to(patient.connectionToken)
        setSessionVariable("NationalPracticeCode").to(patient.odsCode)

        when (gpSystem) {
            "EMIS" -> {
                mockingClient.forEmis {
                    authentication.endUserSessionRequest()
                            .respondWithServiceUnavailable()
                }
            }
            "TPP" -> {
                mockingClient.forTpp {
                    authentication.authenticateRequest(Authenticate())
                            .respondWithServiceUnavailable()
                }
            }
            "VISION" -> {
                mockingClient.forVision {
                    authentication.getConfigurationRequest(
                            VisionMockDefaults.getVisionUserSession(patient))
                            .respondWithServiceUnavailable()
                }
            }
        }
    }

    @Given("^I have logged into (.*) and have a valid session cookie$")
    fun givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)

        Serenity.setSessionVariable(GP_SYSTEM).to(gpSystem)
        SerenityHelpers.setPatient(patient)
        SerenityHelpers.setGpSupplier(gpSystem)

        givenIHaveLoggedInAndHaveAValidSessionCookie()
    }

    @Given("^I have logged in and have a valid session cookie$")
    fun givenIHaveLoggedInAndHaveAValidSessionCookie() {
        val gpSystem = SerenityHelpers.getGpSupplier()
        val patient = SerenityHelpers.getPatientOrNull() ?: Patient.getDefault(gpSystem)
        CitizenIdSessionCreateJourney(mockingClient).createFor(patient)
        SessionCreateJourneyFactory.getForSupplier(gpSystem, mockingClient).createFor(patient)
        sessionVariableCalled<WorkerClient>(WorkerClient::class).authentication
                .postSessionConnection(patient.cidUserSession)
    }

    @And("I allow my session to expire")
    fun andIDelayMyRequestByTheDefaultTime() {
        val delayTime = TimeUnit.MINUTES.toMillis(Config.instance.sessionExpiryMinutes)
        Thread.sleep(delayTime + ADDITIONAL_TIME_TO_DELAY)
    }
}
