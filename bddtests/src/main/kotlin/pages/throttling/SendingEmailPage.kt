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
            webDesktopLocator = "//h1[contains(text(), 'Waiting list')]",
            webMobileLocator = "//h1[contains(text(), 'Waiting list')]",
            androidLocator = null,
            page = this
    )

    val emailFeatureText = HybridPageElement(
            webDesktopLocator = "//h3[contains(text(), 'Get an email when all " +
                    "features are available at your GP surgery')]",
            webMobileLocator = "//h3[contains(text(), 'Get an email when all " +
                    "features are available at your GP surgery')]",
            androidLocator = null,
            page = this
    )

    val emailText = HybridPageElement(
            webDesktopLocator = "//h4[contains(text(), 'Email address')]",
            webMobileLocator = "//h4[contains(text(), 'Email address')]",
            androidLocator = null,
            page = this
    )

    val continueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            webMobileLocator = "//button[contains(text(), 'Continue')]",
            androidLocator = null,
            page = this
    )

    val emailInputField = HybridPageElement(
            webDesktopLocator = "//*[@id='emailInput']",
            webMobileLocator = "//*[@id='emailInput']",
            androidLocator = null,
            page = this
    )

    val backLink = HybridPageElement(
            webDesktopLocator = "//*[@id='backIcon']",
            webMobileLocator = "//*[@id='backIcon']",
            androidLocator = null,
            page = this
    )

    val inLineError = HybridPageElement(
            webDesktopLocator = "//*[@id='error-label']//*[@data-purpose='error']",
            webMobileLocator = "//*[@id='error-label']//*[@data-purpose='error']",
            androidLocator = null,
            page = this,
            helpfulName = "Inline Error"
    )

    val choiceError = HybridPageElement(
            webDesktopLocator = "//*[@id='choice-error-label']//*[@data-purpose='error']",
            webMobileLocator = "//*[@id='choice-error-label']//*[@data-purpose='error']",
            androidLocator = null,
            page = this
    )

    val yesRadioButton = HybridPageElement(
            webDesktopLocator = "//input[@id='choice-yes']",
            webMobileLocator = "//input[@id='choice-yes']",
            androidLocator = null,
            page = this

    )

    val noRadioButton = HybridPageElement(
            webDesktopLocator = "//input[@id='choice-no']",
            webMobileLocator = "//input[@id='choice-no']",
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


