package features.legalAndCookiesHub.stepDefinitions

import com.google.gson.Gson
import io.cucumber.java.en.Given
import io.cucumber.java.en.Then
import io.cucumber.java.en.When
import org.junit.Assert
import org.openqa.selenium.Cookie
import pages.legalAndCookies.LegalAndCookiesPage
import pages.legalAndCookies.LegalAndCookiesManageCookiesPage
import java.net.URI
import java.net.URLDecoder
import java.sql.Date
import java.time.LocalDate

data class AnalyticCookieConsent(
    var updatedConsentRequired: Boolean,
    var areAccepted: Boolean,
    var analyticsCookieAccepted: Boolean
)

class LegalAndCookiesHubStepDefinitions {

    lateinit var legalAndCookiesPage: LegalAndCookiesPage
    lateinit var legalAndCookiesManageCookiesPage: LegalAndCookiesManageCookiesPage

    @Then("^the Legal and cookies Hub page is displayed$")
    fun theLegalAndCookiesHubPageIsDisplayed() {
        legalAndCookiesPage.assertDisplayed()
    }

    @Then("^the Legal and cookies Manage cookies page is displayed$")
    fun theLegalAndCookiesManageCookiesPageIsDisplayed() {
        legalAndCookiesManageCookiesPage.assertDisplayed()
    }

    @When("^I click Manage Cookies$")
    fun clickOnManageCookies() {
        legalAndCookiesPage.clickManageCookies
    }

    @Given("^I add the dummy cookie$")
    fun iAddaDummyCookie() {
        val domain = "." + URI(legalAndCookiesManageCookiesPage.driver.currentUrl).host
        val cookieName = "dummyCookie"
        legalAndCookiesManageCookiesPage.driver.manage().addCookie(
            Cookie(cookieName, "dummyValue", domain, "/",
                Date.valueOf(LocalDate.now().plusDays(1)))
        )
        val encodedCookie =
            legalAndCookiesManageCookiesPage.driver.manage().getCookieNamed("nhso.terms").value
        val analyticsCookie = URLDecoder.decode(encodedCookie, "UTF-8")
        val analyticsCookieConsent =
            Gson().fromJson(analyticsCookie, AnalyticCookieConsent::class.java).analyticsCookieAccepted
        Assert.assertEquals(true, analyticsCookieConsent)
        legalAndCookiesManageCookiesPage.cookieToggle.assertOn()
    }

    @When("^I change the cookie consent toggle to 'on'$")
    fun iChangeTheCookieConsentToggleToOn() {
        legalAndCookiesManageCookiesPage.cookieToggle.assertIsVisible()
        legalAndCookiesManageCookiesPage.cookieToggle.assertOff()
        legalAndCookiesManageCookiesPage.cookieToggle.click()
    }

    @When("^I change the cookie consent toggle to 'off'$")
    fun iChangeTheCookieConsentToggleToOff() {
        legalAndCookiesManageCookiesPage.cookieToggle.assertIsVisible()
        legalAndCookiesManageCookiesPage.cookieToggle.assertOn()
        legalAndCookiesManageCookiesPage.cookieToggle.click()
    }

    @Then("^the dummy cookie is deleted$")
    fun dummyCookieIsDeleted() {
        legalAndCookiesManageCookiesPage.pageBody.waitForElement()
        val cookieName = "dummyCookie"
        val deletedCookie = legalAndCookiesManageCookiesPage.driver.manage().getCookieNamed(cookieName)
        Assert.assertEquals(null, deletedCookie)
    }

    @Then("^I can see the toggle button is set to 'on'$")
    fun iCanSeeToggleButtonSetToOn() {
        legalAndCookiesManageCookiesPage.cookieToggle.assertOn()
    }

    @Then("^I can see the toggle button is set to 'off'$")
    fun iCanSeeToggleButtonSetToOff() {
        legalAndCookiesManageCookiesPage.cookieToggle.assertOff()
    }
}
