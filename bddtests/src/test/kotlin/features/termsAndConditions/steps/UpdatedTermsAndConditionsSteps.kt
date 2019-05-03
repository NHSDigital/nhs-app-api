package features.termsAndConditions.steps

import cucumber.api.java.en.When
import net.thucydides.core.annotations.Step
import pages.UpdatedTermsAndConditionsPage
import pages.navigation.HeaderNative

open class UpdatedTermsAndConditionsSteps {

    lateinit var updatedTermsAndConditionsPage: UpdatedTermsAndConditionsPage
    lateinit var headerNative: HeaderNative
    private val pageHeader = "Updated conditions of use"

    @Step
    fun thePageHeaderIsVisible() {
        headerNative.waitForPageHeaderText(pageHeader)
    }

    @When("^I agree to the updated terms and conditions$")
    fun iAgreeToTheUpdatedTermsAndConditions() {
        updatedTermsAndConditionsPage.updatedTermsAndConditionsLabel.click()
        updatedTermsAndConditionsPage.continueButton.click()
    }

    @Step
    fun mainErrorMessageVisible() : Boolean {
        return updatedTermsAndConditionsPage.isMainErrorMessageVisible()
    }

    @Step
    fun secondaryErrorMessageVisible() : Boolean {
        return updatedTermsAndConditionsPage.isSecondaryErrorMessageVisible()
    }
}

