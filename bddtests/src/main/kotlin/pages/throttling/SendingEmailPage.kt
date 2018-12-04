package pages.throttling

import net.serenitybdd.core.pages.WebElementFacade
import pages.HybridPageObject
import pages.HybridPageElement

class SendingEmailPage : HybridPageObject() {


    companion object {
        val validEmail = "bddTest@nhs.com"
        val invalidEmail = "bddTest"
    }
    val waitingListResultsHeader = HybridPageElement(
            browserLocator = "//h1[contains(text(), 'Waiting list')]",
            androidLocator = null,
            page = this
    )

    val emailFeatureText =
            HybridPageElement(
                    browserLocator = "//h3[contains(text(), 'Get an email when all features are available at your GP " +
                            "surgery')]",
                    androidLocator = null,
                    page = this
            )

    val contactYouText =
            HybridPageElement(
                    browserLocator = "//p[contains(text(), 'We will only contact you about this')]",
                    androidLocator = null,
                    page = this
            )
    val emailText =
            HybridPageElement(
                    browserLocator = "//h4[contains(text(), 'Email')]",
                    androidLocator = null,
                    page = this
            )

    val continueButton = HybridPageElement(
            browserLocator = "//button[contains(text(), 'Continue')]",
            androidLocator = null,
            page = this
    )

    val homeButton = HybridPageElement(
            browserLocator = "//button[contains(text(), 'Go to home screen')]",
            androidLocator = null,
            page = this
    )

     val emailInputField = HybridPageElement(
            browserLocator = "//*[@id='emailInput']",
            androidLocator = null,
            page = this
    )

    val backLink = HybridPageElement(
            browserLocator = "//*[@id='backIcon']",
            androidLocator = null,
            page = this
    )

    fun isWaitingListResultsHeaderVisible(): Boolean {
        return waitingListResultsHeader.element.isDisplayed
    }

    fun isEmailFeatureTextVisible(): Boolean {
        return emailFeatureText.element.isDisplayed
    }

    fun isContactYouTextVisible(): Boolean {
        return contactYouText.element.isDisplayed
    }

    fun isemailTextVisible(): Boolean {
        return emailText.element.isDisplayed
    }
    fun clickContinueButton() {
        continueButton.element.click()
    }

    fun clickHomeButton() {
        homeButton.element.click()
    }

    val inLineError = HybridPageElement(
            browserLocator = "//*[@id='error-label']//*[@data-purpose='error']",
            androidLocator = null,
            page = this,
            helpfulName = "Inline Error"
    )


    fun isInvalidEmailVisible(): Boolean {
        val message = "Enter a valid email address"
        return findByXpath("//span[contains(text(), \"$message\")]").isVisible
    }

    fun enterEmail(searchTerm: String) {
        emailInputField.element.type<WebElementFacade>(searchTerm)
        }
    }


