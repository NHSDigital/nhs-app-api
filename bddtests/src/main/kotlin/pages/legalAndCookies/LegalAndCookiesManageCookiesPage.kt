package pages.legalAndCookies

import io.cucumber.java.en.Then
import net.thucydides.core.annotations.DefaultUrl
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.ToggleElement

@DefaultUrl("http://web.local.bitraft.io:3000/more/account-and-settings/legal-and-cookies/manage-cookies/")
class LegalAndCookiesManageCookiesPage : HybridPageObject() {

    val pageBody = HybridPageElement(
        webDesktopLocator = "/html/body",
        iOSLocator = "/html/body",
        androidLocator = "/html/body",
        page = this
    )

    val cookiesPolicy = LegalAndCookiesManageCookiePolicyModule(this)

    val cookieToggle = ToggleElement(this, "Allow optional analytic cookies", "allow_cookies")

    fun assertDisplayed() {
        cookiesPolicy.cookiePolicy.assertSingleElementPresent()
        cookieToggle.assertIsVisible()
    }

    @Then("^I can see the toggle button is set to 'on'$")
    fun iCanSeeToggleButtonSetToOn() {
        cookieToggle.assertOn()
    }

    @Then("^I can see the toggle button is set to 'off'$")
    fun iCanSeeToggleButtonSetToOff() {
        cookieToggle.assertOff()
    }
}

