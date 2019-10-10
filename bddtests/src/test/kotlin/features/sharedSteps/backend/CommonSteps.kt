package features.sharedSteps.backend

import config.Config
import cucumber.api.java.After
import cucumber.api.java.Before
import cucumber.api.java.en.And
import cucumber.api.java.en.Given
import mocking.MockingClient
import mocking.defaults.dataPopulation.journies.session.CitizenIdSessionCreateJourney
import mocking.defaults.dataPopulation.journies.session.SessionCreateJourneyFactory
import models.Patient
import net.serenitybdd.core.Serenity.getCurrentSession
import net.serenitybdd.core.Serenity.getWebdriverManager
import net.serenitybdd.core.Serenity.sessionVariableCalled
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import org.openqa.selenium.WebDriver
import pages.WEB_CONTEXT
import utils.GlobalSerenityHelpers
import utils.SerenityHelpers
import utils.contains
import utils.set
import webdrivers.getMobileDriver
import webdrivers.isAndroid
import webdrivers.isIOS
import worker.WorkerClient
import java.util.concurrent.TimeUnit
import java.util.logging.Level

private const val ADDITIONAL_TIME_TO_DELAY = 10
val CONSOLE_LOG_STRINGS_TO_IGNORE =
        arrayOf("favicon.ico"
                ,"redirectToCitizenId"
                ,"Failed to load resource"
                ,"Request failed with status code 50"
                ,"https://assets.nhs.uk/fonts")

class CommonSteps : AbstractSteps() {

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
        var driver = getWebdriverManager().currentDriver

        if (driver != null && (driver.isAndroid() || driver.isIOS())) {
            driver = switchWebview(driver)
        }

        if (driver != null) {
            val logs = driver.manage().logs().get("browser")
                    .filter { it.level == Level.SEVERE }
                    .filterNot { it.message.contains(CONSOLE_LOG_STRINGS_TO_IGNORE) }

            Assert.assertTrue("There should not be any console logs but found: \r\n $logs",
                    logs.isEmpty())
        }
    }

    private fun switchWebview(currentDriver: WebDriver): WebDriver? {
        var driver = currentDriver
        driver = driver.getMobileDriver()
        if (driver.context.contains(WEB_CONTEXT, ignoreCase = true)) {
            println("Already in $WEB_CONTEXT context: ${driver.context}")
        } else {
            for (context in driver.contextHandles) {
                if (context.contains(WEB_CONTEXT, true)) {
                    println("Switching context to $context... Currently on: ${driver.context}")
                    driver.context(context)
                    println("Switched context! Now on: ${driver.context}")
                    break
                }
            }
        }
        return driver
    }

    @Given("^I have logged into (.*) and have a valid session cookie$")
    fun givenIHaveLoggedIntoXAndHaveAValidSessionCookie(gpSystem: String) {
        val patient = Patient.getDefault(gpSystem)

        GlobalSerenityHelpers.GP_SYSTEM.set(gpSystem)
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
