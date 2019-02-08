package pages.loggedOut

import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue
import pages.HybridPageElement
import pages.HybridPageObject

@DefaultUrl("http://web.local.bitraft.io:3000/login")
class LoginPage : HybridPageObject() {

    private val symptomsButtonHeading = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'How are you feeling today?')]",
            webMobileLocator = "//h2[contains(text(), 'How are you feeling today?')]",
            androidLocator = null,
            page = this
    )

    val symptomsButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_home_symptoms']",
            webMobileLocator = "//*[@id='btn_home_symptoms']",
            androidLocator = null,
            page = this
    )

    private val loginOrCreateAccountButtonHeading = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'To access your NHS services')]",
            webMobileLocator = "//h2[contains(text(), 'To access your NHS services')]",
            androidLocator = null,
            page = this
    )

    val loginOrCreateAccountButton = HybridPageElement(
            webDesktopLocator = "//*[@data-id='login-button']",
            webMobileLocator = "//*[@data-id='login-button']",
            androidLocator = null,
            page = this
    )

    val throttlingNotParticipatingHeader = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'More features will be coming soon to this GP surgery')]",
            webMobileLocator = "//h2[contains(text(), 'More features will be coming soon to this GP surgery')]",
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

        assertTrue("Symptoms button header is not displayed correctly.",
                symptomsButtonHeaderIsDisplayed())
        assertTrue("Symptoms button is not displayed correctly.",
                symptomsButtonIsDisplayed())
        assertTrue("'Log in or create account' button header is not displayed correctly.",
                loginOrCreateAccountButtonHeaderIsDisplayed())
        assertTrue("'Log in or create account' button is not displayed correctly.",
                loginOrCreateAccountButtonIsDisplayed())
    }

    private fun symptomsButtonHeaderIsDisplayed() = symptomsButtonHeading.element.isDisplayed

    private fun symptomsButtonIsDisplayed() = symptomsButton.element.isDisplayed

    private fun loginOrCreateAccountButtonHeaderIsDisplayed() = loginOrCreateAccountButtonHeading.element.isDisplayed

    private fun loginOrCreateAccountButtonIsDisplayed() = loginOrCreateAccountButton.element.isDisplayed

    fun helpIconIsVisible() = helpIcon.element.isVisible

    // Checks to see the menu item is not present on the page.
    fun assertMenuIsNotVisible() {
        assertFalse(findByXpath("//nav[@class='menu']").isVisible)
    }

    fun assertTimeoutBannerIsVisible() {
        assertTrue("Expected timeout banner to be visible", timeoutBanner.element.isVisible)
    }
}
