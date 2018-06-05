package features.sharedSteps

import net.serenitybdd.core.SerenitySystemProperties
import net.serenitybdd.core.exceptions.SerenityManagedException
import net.thucydides.core.ThucydidesSystemProperty
import net.thucydides.core.annotations.Step
import org.junit.Assert
import pages.LoginPage
import java.net.MalformedURLException
import java.net.URL
import java.util.NoSuchElementException

open class BrowserSteps {

    lateinit var loginPage: LoginPage

    @Step
    open fun goToApp() {
        loginPage.open()
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

    @Step()
    open fun CheckLoginDetailsAreReset() {
        val driver = loginPage.driver
        var nhso =driver.manage().cookies.first { x -> x.name == "nhso" }.toString();

        // No user details...
        Assert.assertEquals(-1, nhso.indexOf("familyName"));
        Assert.assertEquals(-1, nhso.indexOf("givenName"));
        Assert.assertEquals(-1, nhso.indexOf("userSession"));
        Assert.assertTrue("user details should be blank", nhso.indexOf("%22user%22:{}") > 0);
        Assert.assertTrue("No one is log in", nhso.indexOf("%22loggedIn%22:false") > 0);

        // No remaining personal data left...
        Assert.assertTrue("Appointments should be blank", nhso.indexOf("{%22appointmentSlots%22:{%22appointmentSessions%22:[]%2C%22clinicians%22:[]%2C%22locations%22:[]%2C%22slots%22:[]%2C%22hasLoaded%22:false%2C%22hasErrored%22:false}") > 0);
        Assert.assertTrue("Prescriptions should be blank", nhso.indexOf("%22prescriptions%22:{%22prescriptionCourses%22:[]%2C%22hasLoaded%22:false%2C%22hasErrored%22:false}") > 0);
        Assert.assertTrue("Repeat prescriptions should be blank", nhso.indexOf("%22repeatPrescriptionCourses%22:{%22repeatPrescriptionCourses%22:[]%2C%22hasLoaded%22:false%2C%22hasErrored%22:false}") > 0);

    }

    @Step
    fun changeTab(url: URL) {
        val driver = loginPage.driver

        var allWindows: MutableList<String> = mutableListOf()

        for (window in loginPage.driver.windowHandles) {
            driver.switchTo().window(window)
            allWindows.add(0, driver.currentUrl)
            if (url.host.equals(URL(driver.currentUrl).host)) {
                return
            }
        }

        throw NoSuchElementException("No tab found with $url. All windows: ${allWindows.reversed()}")
    }

    @Step
    fun changeTabToApp() {
        val baseUrl: String = SerenitySystemProperties.getProperties().getValue(ThucydidesSystemProperty.WEBDRIVER_BASE_URL)
        try{
            changeTab(URL(baseUrl))
        } catch (e: MalformedURLException) {
            throw SerenityManagedException("Malformed URL from ${ThucydidesSystemProperty.WEBDRIVER_BASE_URL}: $baseUrl", e)
        }
    }

    @Step
    fun refreshPage() {
        loginPage.driver.navigate().refresh()
    }
}
