package pages.loggedOut

import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import pages.HybridPageElement
import pages.HybridPageObject
import pages.isDisplayed
import pages.isVisible

@DefaultUrl("http://web.local.bitraft.io:3000/login")
class LoginPage : HybridPageObject() {

    val downloadAppPanel = HybridPageElement(
             webDesktopLocator = "//div[@data-id='app-panel']",
            webMobileLocator = "//div[@data-id='app-panel']",
            androidLocator = null,
            page = this
    )

    val beforeYouStartDiv = HybridPageElement(
            webDesktopLocator = "//div[@id='before-you-start']",
            webMobileLocator = "//div@id='before-you-start']",
            androidLocator = null,
            page = this
    )

    val otherServicesDiv = HybridPageElement(
            webDesktopLocator = "//div[@id='other-services']",
            webMobileLocator = "//div@id='other-services']",
            androidLocator = null,
            page = this
    )

    val loginOrCreateAccountButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue with NHS login')]",
            webMobileLocator = "//button[contains(text(), 'Continue with NHS login')]",
            androidLocator = null,
            page = this
    )

    private val helpIcon = HybridPageElement(
            webDesktopLocator = "//a[@id='help_icon']/*[name()='svg']",
            androidLocator = null,
            page = this
    )

    private lateinit var accountCreationPage: CIDAccountCreationPage

    private val timeoutBanner = HybridPageElement(
            webDesktopLocator = "//*[@data-purpose='session-timeout'][contains(text(), " +
                    "'For your security, you need to log in again')]",
            androidLocator = null,
            page = this
    )

    fun signIn() {
        loginOrCreateAccountButton.click()
    }

    fun createAccount(patient: Patient) {
        loginOrCreateAccountButton.click()
        accountCreationPage.completeAccountCreation(patient)
    }

    fun clickHelpIcon() {
        helpIcon.click()
    }

    override fun shouldBeDisplayed() {
        super.shouldBeDisplayed()

        assertTrue("Other Services information is not displayed correctly.",
                otherServicesDivIsDisplayed())
        assertTrue("Before you start information is not displayed correctly.",
                beforeYouStartDivIsDisplayed())
        assertTrue("Download app panel is not displayed correctly.",
                downloadAppPanelIsDisplayed())
        assertTrue("'Continue with NHS login' button is not displayed correctly.",
                loginOrCreateAccountButtonIsDisplayed())
    }

    private fun loginOrCreateAccountButtonIsDisplayed() = loginOrCreateAccountButton.isDisplayed

    private fun otherServicesDivIsDisplayed() = otherServicesDiv.isDisplayed

    private fun beforeYouStartDivIsDisplayed() = beforeYouStartDiv.isDisplayed

    private fun downloadAppPanelIsDisplayed() = downloadAppPanel.isDisplayed

    fun helpIconIsVisible() = helpIcon.isVisible

    // Checks to see the menu item is not present on the page.
    fun assertMenuIsNotVisible() {
        assertFalse(findByXpath("//nav[@class='menu']").isVisible)
    }

    fun assertTimeoutBannerIsVisible() {
        assertTrue("Expected timeout banner to be visible", timeoutBanner.isVisible)
    }
}
