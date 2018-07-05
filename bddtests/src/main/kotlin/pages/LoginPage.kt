package pages

import mocking.defaults.MockDefaults
import models.Patient
import net.serenitybdd.core.annotations.findby.FindBy
import net.serenitybdd.core.annotations.findby.How
import net.serenitybdd.core.pages.WebElementFacade
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.openqa.selenium.support.ui.WebDriverWait
import java.time.Duration


@DefaultUrl("http://localhost:3000/login")
class LoginPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val heading = HybridPageElement(
            browserLocator = "//header/h1[contains(text(), 'Home')]",
            androidLocator = null,
            page = this
    )

    val symptomsButton = HybridPageElement(
            browserLocator = "//*[@id='btn_home_symptoms']",
            androidLocator = null,
            page = this
    )

    val loginButton = HybridPageElement(
            browserLocator = "//*[@data-id='login-button']",
            androidLocator = null,
            page = this
    )

    val createAccountButton = HybridPageElement(
            browserLocator = "//*[@data-id='create-account-button']",
            androidLocator = null,
            page = this
    )

    lateinit var accountCreationPage : CIDAccountCreationPage

    val timeoutBanner = HybridPageElement(
            browserLocator = "//div[@class='alertContainer']/h3[contains(text(), 'Session Expired. Please sign in again.')]",
            androidLocator = null,
            page = this
    )

    fun checkMySymptoms() {
        symptomsButton.element.click()
    }

    fun signIn(patient: Patient = MockDefaults.patient) {
        loginButton.element.click()

        findByXpath("//input[@name='mock_patient']").sendKeys(patient.hashCode().toString())
        findByXpath("//input[@type='submit']").click()
    }

    fun createAccount(patient: Patient) {
        clickCreateAccountButton()
        accountCreationPage.completeAccountCreation(patient)
    }

    fun isCreateAccountButtonVisible() : Boolean {
        return createAccountButton.element.isVisible
    }

    fun clickCreateAccountButton()
    {
        createAccountButton.element.click()
    }

    override fun shouldBeDisplayed() {
        super.shouldBeDisplayed()

        Assert.assertTrue("Heading was not displayed.", headingIsDisplayed())
        Assert.assertTrue("Buttons were not displayed.", buttonsAreDisplayed())
    }

    private fun buttonsAreDisplayed(): Boolean {
        return symptomsButton.element.isDisplayed
                && loginButton.element.isDisplayed
                && createAccountButton.element.isDisplayed
    }

    private fun headingIsDisplayed(): Boolean {
        return heading.element.isDisplayed
    }
    // Checks to see the menu item is not present on the page.
    fun assertMenuIsNotVisible() {
        Assert.assertFalse(findByXpath("//nav[@class='menu']").isVisible);
    }

    fun assertTimeoutBannerIsVisible() {
        Assert.assertTrue("Expected timeout banner to be visible" ,timeoutBanner.element.isVisible)
    }
}
