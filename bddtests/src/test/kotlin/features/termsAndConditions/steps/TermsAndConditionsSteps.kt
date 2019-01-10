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
    fun termsOfUseLinkVisible() : Boolean {
        return termsAndConditionsPage.isTermsOfUseLinkVisible()
    }
    @Step
    fun privacyPolicyLinkVisible() : Boolean {
        return termsAndConditionsPage.isPrivacyPolicyLinkVisible()
    }
    @Step
    fun cookiesPolicyLinkVisible() : Boolean {
        return termsAndConditionsPage.isCookiesPolicyLinkVisible()
    }
    @Step
    fun secondaryErrorMessageVisible() : Boolean {
        return termsAndConditionsPage.isSecondaryErrorMessageVisible()
    }
    @Step
    fun tcCheckBoxVisible() : Boolean {
        return termsAndConditionsPage.isTcCheckBoxVisible()
    }
    @Step
    fun continueButtonVisible() : Boolean {
        return termsAndConditionsPage.isContinueButtonVisible()
    }

}

