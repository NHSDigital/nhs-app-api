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

    val emailFeatureText = HybridPageElement(
        browserLocator = "//h3[contains(text(), 'Get an email when all features are available at your GP surgery')]",
        androidLocator = null,
        page = this
    )

    val emailText = HybridPageElement(
        browserLocator = "//h4[contains(text(), 'Email address')]",
        androidLocator = null,
        page = this
    )

    val continueButton = HybridPageElement(
        browserLocator = "//button[contains(text(), 'Continue')]",
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

    val inLineError = HybridPageElement(
        browserLocator = "//*[@id='error-label']//*[@data-purpose='error']",
        androidLocator = null,
        page = this,
        helpfulName = "Inline Error"
    )

    val choiceError = HybridPageElement(
        browserLocator = "//*[@id='choice-error-label']//*[@data-purpose='error']",
        androidLocator = null,
        page = this
    )

    val yesRadioButton = HybridPageElement(
        browserLocator = "//input[@id='choice-yes']",
        androidLocator = null,
        page = this
    )

    val noRadioButton = HybridPageElement(
        browserLocator = "//input[@id='choice-no']",
        androidLocator = null,
        page = this
    )

    fun isInvalidEmailVisible(): Boolean {
        val message = "Enter a valid email address"
        return findByXpath("//span[contains(text(), \"$message\")]").isVisible
    }

    fun enterEmail(searchTerm: String) {
        emailInputField.element.type<WebElementFacade>(searchTerm)
    }
}


