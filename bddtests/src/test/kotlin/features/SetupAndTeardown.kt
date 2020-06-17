package features

import config.Config
import cucumber.api.Scenario
import cucumber.api.java.After
import cucumber.api.java.Before
import mocking.MockingClient
import mongodb.MongoDBConnection
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
import webdrivers.browserstack.BrowserstackLocalService
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
                ,"https://assets.nhs.uk/fonts"
                ,"unknown action type")

class SetupAndTeardown {

    private val mockingClient = MockingClient.instance

    @Before
    fun beforeEachScenario(scenario: Scenario) {
        getCurrentSession().clear()
        MongoDBConnection.collections().forEach { connection -> connection.clearCache() }

        val workerClient = WorkerClient()
        mockingClient.wiremockHelper.clearWiremock()

        setSessionVariable(MockingClient::class).to(mockingClient)
        setSessionVariable(WorkerClient::class).to(workerClient)
        GlobalSerenityHelpers.MOCK_NATIVE_LOGIN.set(false)
        GlobalSerenityHelpers.LOGIN_REDIRECT_URI.set(Config.instance.cidRedirectUri)
        GlobalSerenityHelpers.SCENARIO_TITLE.set(scenario.name)
    }

    @After
    @Suppress("TooGenericExceptionCaught") // Exception is re-thrown
    fun afterEachScenario(scenario: Scenario) {
        try {
            executeTearDownActions()
            assertNoConsoleLogs(scenario)
        } catch (e: Exception) {
            // If an exception is thrown from this method serenity fails to close the driver
            getWebdriverManager().closeAllDrivers()

            throw e
        }
    }

    @After
    fun stopBrowserstackIfRunning() {
        BrowserstackLocalService.stop()
    }

    private fun assertNoConsoleLogs(scenario: Scenario) {
        var driver = getWebdriverManager().currentDriver

        if (driver != null && (driver.isAndroid() || driver.isIOS())) {
            driver = switchWebview(driver)
        }

        if (driver != null) {
            val logs = driver.manage().logs().get("browser")?.toList() ?: emptyList()

            if (scenario.isFailed) {
                println("Console logs:\r\n$logs")
            } else {
                val errorLogs = logs
                        .filter { it.level == Level.SEVERE }
                        .filterNot { it.message.contains(CONSOLE_LOG_STRINGS_TO_IGNORE) }

                Assert.assertTrue(
                        "There should not be any console logs but found:\r\n$errorLogs",
                        errorLogs.isEmpty())
            }
        }
    }

    private fun executeTearDownActions() {
        val tearDownActions = GlobalSerenityHelpers.TEAR_DOWN_ACTIONS.getOrNull<List<() -> Unit>>()
        tearDownActions?.forEach { tearDownAction -> tearDownAction.invoke() }
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
