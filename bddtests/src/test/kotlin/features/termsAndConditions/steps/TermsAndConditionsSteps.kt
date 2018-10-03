package features.termsAndConditions.steps

import net.thucydides.core.annotations.Step
import pages.TermsAndConditionsPage

open class TermsAndConditionsSteps {

    lateinit var termsAndConditions: TermsAndConditionsPage

    @Step
    fun mainErrorMessageVisible() : Boolean {
        return termsAndConditions.isMainErrorMessageVisible()
    }
    @Step
    fun mainBodyTextVisible() : Boolean {
        return termsAndConditions.isMainBodyTextVisible()
    }
    @Step
    fun termsOfUseLinkVisible() : Boolean {
        return termsAndConditions.isTermsOfUseLinkVisible()
    }
    @Step
    fun privacyPolicyLinkVisible() : Boolean {
        return termsAndConditions.isPrivacyPolicyLinkVisible()
    }
    @Step
    fun cookiesPolicyLinkVisible() : Boolean {
        return termsAndConditions.isCookiesPolicyLinkVisible()
    }
    @Step
    fun secondaryErrorMessageVisible() : Boolean {
        return termsAndConditions.isSecondaryErrorMessageVisible()
    }
    @Step
    fun tcCheckBoxVisible() : Boolean {
        return termsAndConditions.isTcCheckBoxVisible()
    }
    @Step
    fun continueButtonVisible() : Boolean {
        return termsAndConditions.isContinueButtonVisible()
    }

}

