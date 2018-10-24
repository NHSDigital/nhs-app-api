package pages

import mocking.defaults.EmisMockDefaults
import mocking.defaults.MockDefaults
import models.Patient
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.junit.Assert.assertFalse
import org.junit.Assert.assertTrue

@DefaultUrl("http://web.local.bitraft.io:3000/login")
class LoginPage : HybridPageObject() {

    val symptomsButtonHeading = HybridPageElement(
            browserLocator = "//h2[contains(text(), 'How are you feeling today?')]",
            androidLocator = null,
            page = this
    )

    val symptomsButton = HybridPageElement(
            browserLocator = "//*[@id='btn_home_symptoms']",
            androidLocator = null,
            page = this
    )

    val loginOrCreateAccountButtonHeading = HybridPageElement(
            browserLocator = "//h2[contains(text(), 'To access your GP services')]",
            androidLocator = null,
            page = this
    )

    val loginOrCreateAccountButton = HybridPageElement(
            browserLocator = "//*[@data-id='login-button']",
            androidLocator = null,
            page = this
    )

    lateinit var accountCreationPage: CIDAccountCreationPage

    private val timeoutBanner = HybridPageElement(
            browserLocator = "//*[@data-purpose='session-timeout'][contains(text(), " +
                             "'For your security, you need to log in again')]",
            androidLocator = null,
            page = this
    )

    fun signIn() {
        loginOrCreateAccountButton.element.click()
    }

    fun createAccount(patient: Patient) {
        loginOrCreateAccountButton.element.click()
        accountCreationPage.completeAccountCreation(patient)
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

    // Checks to see the menu item is not present on the page.
    fun assertMenuIsNotVisible() {
        assertFalse(findByXpath("//nav[@class='menu']").isVisible)
    }

    fun assertTimeoutBannerIsVisible() {
        assertTrue("Expected timeout banner to be visible", timeoutBanner.element.isVisible)
    }
}
