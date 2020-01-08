package features.termsAndConditions.steps

import net.thucydides.core.annotations.Step
import pages.TermsAndConditionsPage

open class TermsAndConditionsSteps {

    lateinit var termsAndConditionsPage: TermsAndConditionsPage

    @Step
    fun mainErrorMessageVisible() : Boolean {
        return termsAndConditionsPage.isMainErrorMessageVisible()
    }
    @Step
    fun mainBodyTextVisible() : Boolean {
        return termsAndConditionsPage.isMainBodyTextVisible()
    }
    @Step
    fun secondaryErrorMessageVisible() : Boolean {
        return termsAndConditionsPage.isSecondaryErrorMessageVisible()
    }
    @Step
    fun assertTcCheckBoxVisible() {
        termsAndConditionsPage.assertTcCheckBoxVisible()
    }
    @Step
    fun continueButtonVisible() : Boolean {
        return termsAndConditionsPage.isContinueButtonVisible()
    }

}

