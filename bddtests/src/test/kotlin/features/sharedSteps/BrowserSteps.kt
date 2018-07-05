package features.sharedSteps

import net.serenitybdd.core.SerenitySystemProperties
import net.serenitybdd.core.exceptions.SerenityManagedException
import net.thucydides.core.ThucydidesSystemProperty
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.LoginPage
import java.net.MalformedURLException
import java.net.URL
import java.util.*

open class BrowserSteps {

    lateinit var loginPage: LoginPage

    @Step
    open fun goToApp() {
        if (!loginPage.onMobile()) {
            loginPage.open()
        }
    }

    @Step
    open fun browseTo(url: String) {
        loginPage.driver.get(url)
    }

    @Step
    open fun shouldHaveTitle(title: String) {
        Assert.assertEquals(title, loginPage.title)
    }

    @Step
    open fun shouldHaveUrl(url: String) {
        Assert.assertEquals(url, loginPage.driver.currentUrl)
    }

    private fun fetchCookieContents(cookieName: String): String {
        val driver = loginPage.driver
        var cookieValue = driver.manage().cookies.first { x -> x.name == cookieName }.toString()
        cookieValue = cookieValue.replace("%22", "'")
        cookieValue = cookieValue.replace("%2C", ",")
        return cookieValue
    }

    // checks whether or not he names cookie contains the specified contents in it's value.
    // note: the codes %22 (') and %2C (,) are replaced with real charactors to make the test strings easier to read.
    private fun cookieContains(cookieName: String, content: String): Boolean {
        var cookieValue = fetchCookieContents(cookieName)
        return cookieValue.indexOf(content) > 0;
    }

    private fun assertCookieContains(errorMessage: String, cookieName: String, content: String) {
        if (cookieContains(cookieName, content) == false) {
            var actual = fetchCookieContents(cookieName)
            var test = actual.contains(content)
            Assert.assertTrue(errorMessage + "\nExpected to contain: '$content'\nActual: '$actual'", test)
        }
    }

    @Step()
    open fun checkLoginDetailsAreReset() {
        val vuexCookieName = "nhso";

        // No user details...
        Assert.assertFalse(cookieContains(vuexCookieName, "name"));
        Assert.assertFalse(cookieContains(vuexCookieName, "userSession"));
        Assert.assertTrue("user details should be blank", cookieContains(vuexCookieName, "'user':{}"));
        Assert.assertTrue("No one is logged in", cookieContains(vuexCookieName, "'loggedIn':false"));

        // No remaining personal data left...
        assertCookieContains("Appointments should be blank", vuexCookieName, "'appointmentSlots':{'appointmentSessions':[],'clinicians':[],'locations':[],'slots':[],'hasLoaded':false,'hasErrored':false,'selectedSlot':null}");
        assertCookieContains("Prescriptions should be blank", vuexCookieName, "'prescriptions':{'prescriptionCourses':null,'hasLoaded':false,'hasErrored':false}");
        assertCookieContains("Repeat prescriptions should be blank", vuexCookieName, "'repeatPrescriptionCourses':{'courses':[],'repeatPrescriptionCourses':[],'specialRequest':null,'justOrderedARepeatPrescription':false,'hasLoaded':false,'hasErrored':false,'validated':false,'isValid':false}")
    }

    @Step
    fun changeTab(url: URL) {
        val driver = loginPage.driver

        var allWindows: MutableList<String> = mutableListOf()

        for (window in loginPage.driver.windowHandles) {
            driver.switchTo().window(window)
            allWindows.add(0, driver.currentUrl)
            if (url.host == URL(driver.currentUrl).host) {
                return
            }
        }

        throw NoSuchElementException("No tab found with $url. All windows: ${allWindows.reversed()}")
    }

    @Step
    fun changeTabToApp() {
        val baseUrl: String = SerenitySystemProperties.getProperties().getValue(ThucydidesSystemProperty.WEBDRIVER_BASE_URL)
        try {
            changeTab(URL(baseUrl))
        } catch (e: MalformedURLException) {
            val message = "Malformed URL from ${ThucydidesSystemProperty.WEBDRIVER_BASE_URL}: $baseUrl"
            println("ERROR:")
            println(message)
            throw SerenityManagedException(message, e)
        }
    }

    @Step
    fun refreshPage() {
        loginPage.driver.navigate().refresh()
    }
}

