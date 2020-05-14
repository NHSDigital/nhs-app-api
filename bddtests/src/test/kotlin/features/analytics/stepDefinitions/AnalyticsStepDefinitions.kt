package features.analytics.stepDefinitions

import cucumber.api.java.en.Given
import org.junit.Assert
import org.openqa.selenium.JavascriptExecutor
import pages.HomePage

class AnalyticsStepDefinitions {

    lateinit var homePage: HomePage

    @Given("^the analytics data object is available$")
    fun analyticsDataObjectIsAvailable() {
        Assert.assertTrue("Expected the analytics data object (window.digitalData) to be available.",
                dataObjectIsAvailable())
    }

    @Suppress("TooGenericExceptionCaught", "Any exception thrown from javascript")
    private fun dataObjectIsAvailable(): Boolean {
        val dataObject = try {
            digitalData()
        } catch (e: Exception) {
            null
        }

        return dataObject != null
    }

    private fun digitalData(): Any {
        val jsExecutor = homePage.driver as JavascriptExecutor
        return jsExecutor.executeScript("return window.digitalData") as Any
    }
}
