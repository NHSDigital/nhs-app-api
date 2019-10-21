package features

import cucumber.api.java.After
import cucumber.api.java.Before
import mocking.MockingClient
import net.serenitybdd.core.Serenity.getCurrentSession
import net.serenitybdd.core.Serenity.getWebdriverManager
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import org.openqa.selenium.WebDriver
import pages.WEB_CONTEXT
import utils.contains
import webdrivers.getMobileDriver
import webdrivers.isAndroid
import webdrivers.isIOS
import worker.WorkerClient
import java.util.logging.Level

val CONSOLE_LOG_STRINGS_TO_IGNORE =
        arrayOf("favicon.ico"
                ,"redirectToCitizenId"
                ,"Failed to load resource"
                ,"Request failed with status code 50"
                ,"https://assets.nhs.uk/fonts")

class SetupAndTeardown {

    private val mockingClient = MockingClient.instance

    @Before
    fun beforeEachScenario() {
        getCurrentSession().clear()

        val workerClient = WorkerClient()
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
}

