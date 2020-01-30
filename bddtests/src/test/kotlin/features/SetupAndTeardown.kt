package features

import config.Config
import cucumber.api.java.After
import cucumber.api.java.Before
import mocking.MockingClient
import net.serenitybdd.core.Serenity.getCurrentSession
import net.serenitybdd.core.Serenity.getWebdriverManager
import net.serenitybdd.core.Serenity.setSessionVariable
import org.junit.Assert
import org.openqa.selenium.WebDriver
import pages.WEB_CONTEXT
import utils.GlobalSerenityHelpers
import utils.contains
import utils.getOrNull
import utils.set
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
                ,"Uncaught (in promise)"
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
        GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(false)
        GlobalSerenityHelpers.LOGIN_REDIRECT_URI.set(Config.instance.cidRedirectUri)
    }

    @After
    fun afterEachScenario() {

        val tearDownActions = GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.getOrNull<List<() -> Unit>>()
        tearDownActions?.forEach { tearDownAction -> tearDownAction.invoke() }

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

