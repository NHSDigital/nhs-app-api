package features.manageCookies.stepDefintions

import com.google.gson.Gson
import cucumber.api.java.en.Given
import cucumber.api.java.en.Then
import cucumber.api.java.en.When
import org.junit.Assert
import org.openqa.selenium.Cookie
import pages.HybridPageObject
import pages.manageCookies.ManageCookiesPage
import java.net.URI
import java.net.URLDecoder
import java.time.LocalDate

data class AnalyticCookieConsent(
        var updatedConsentRequired: Boolean,
        var areAccepted: Boolean,
        var analyticsCookieAccepted: Boolean
)

class ManageCookiesStepDefinitions {

    lateinit var manageCookies: ManageCookiesPage
    lateinit var genericPage: HybridPageObject

    @Then("^the Cookies page is displayed$")
    fun theAccountPageIsDisplayed() {
        manageCookies.assertDisplayed()
    }

    @Given("^I add the dummy cookie$")
    fun iAddaDummyCookie() {
        val domain = "." + URI(manageCookies.driver.currentUrl).host
        val cookieName = "dummyCookie"
        manageCookies.driver.manage().addCookie(
                Cookie(cookieName, "dummyValue", domain, "/",
                        java.sql.Date.valueOf(LocalDate.now().plusDays(1))))
        val encodedCookie =
                manageCookies.driver.manage().getCookieNamed("nhso.terms").value
        val analyticsCookie = URLDecoder.decode(encodedCookie, "UTF-8")
        val analyticsCookieConsent =
                Gson().fromJson(analyticsCookie, AnalyticCookieConsent::class.java).analyticsCookieAccepted
        Assert.assertEquals(true, analyticsCookieConsent)
        manageCookies.cookieToggle.assertOn()
    }

    @When("^I click the change consent toggle$")
    fun iClickTheChangeConsentToggle() {
        manageCookies.cookieToggle.click()
    }

    @When("^I click the cookies link$")
    fun iClickTheCookiesLink() {
        manageCookies.cookiesLink.click()
    }

    @Then("^the dummy cookie is deleted$")
    fun dummyCookieIsDeleted() {
        manageCookies.pageBody.waitForElement()
        val cookieName = "dummyCookie"
        val deletedCookie = manageCookies.driver.manage().getCookieNamed(cookieName)
        Assert.assertEquals(null, deletedCookie)
    }

    @Then("^I can see the toggle button to change my current consent$")
    fun iCanSeeToggleButton() {
        manageCookies.cookieToggle.assertOn()
    }
}
