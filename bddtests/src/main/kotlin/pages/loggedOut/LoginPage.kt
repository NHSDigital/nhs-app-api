package pages.loggedOut

import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert.assertFalse
import pages.HybridPageElement
import pages.HybridPageObject
import pages.assertIsDisplayed
import pages.assertIsVisible

@DefaultUrl("http://web.local.bitraft.io:3000/login")
class LoginPage : HybridPageObject() {

    private val pageHeading = HybridPageElement(
        webDesktopLocator = "//h1[contains(text(), 'Access your NHS services')]",
        page = this
    )

    val downloadAppPanel = HybridPageElement(
        webDesktopLocator = "//div[@data-id='app-panel']",
        page = this
    )

    val desktopSpecificInformation = HybridPageElement(
        webDesktopLocator = "//*[@id='desktopSpecificInformation']",
        page = this
    )

    val beforeYouStartDiv = HybridPageElement(
        webDesktopLocator = "//div[@id='before-you-start']",
        page = this
    )

    val otherServicesDiv = HybridPageElement(
        webDesktopLocator = "//div[@id='other-services']",
        page = this
    )

    val loginOrCreateAccountButton = HybridPageElement(
        webDesktopLocator = "//button[contains(text(), 'Continue with NHS login')]",
        page = this
    )

    private val helpIcon = HybridPageElement(
        webDesktopLocator = "//a[@id='help_icon']/*[name()='svg']",
        page = this
    )

    private lateinit var accountCreationPage: CIDAccountCreationPage

    private val timeoutBanner = HybridPageElement(
        webDesktopLocator = "//*[@data-purpose='session-timeout'][contains(text(), " +
                "'For your security, you need to log in again')]",
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
        pageHeading.assertIsDisplayed("Page heading is not displayed correctly.")
    }

    fun helpIconIsVisible() = helpIcon.assertIsVisible()

    // Checks to see the menu item is not present on the page.
    fun assertMenuIsNotVisible() {
        assertFalse(findByXpath("//nav[@class='menu']").isVisible)
    }

    fun assertTimeoutBannerIsVisible() {
        timeoutBanner.assertIsVisible("Expected timeout banner to be visible")
    }
}

