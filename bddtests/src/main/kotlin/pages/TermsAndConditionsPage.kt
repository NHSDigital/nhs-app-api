package pages

import net.thucydides.core.annotations.DefaultUrl

@Suppress("TooManyFunctions")
@DefaultUrl("http://web.local.bitraft.io:3000/terms-and-conditions")
class TermsAndConditionsPage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val mainErrorMessage = HybridPageElement(
            browserLocator = "//*[@id='error_msg']",
            androidLocator = null,
            page = this
    )

    val mainBodyText = HybridPageElement(
            browserLocator = "//*[@id='text_body']",
            androidLocator = null,
            page = this
    )

    val secondaryErrorMessage = HybridPageElement(
            browserLocator = "//*[@id='error_txt']",
            androidLocator = null,
            page = this
    )

    val tcCheckBox = HybridPageElement(
            browserLocator = "//*[@id='agree_checkbox']",
            androidLocator = null,
            page = this
    )

    val continueButton = HybridPageElement(
            browserLocator = "//*[@id='btn_accept']",
            androidLocator = null,
            page = this
    )

    val termsOfUseLink = HybridPageElement(
            browserLocator = "//a[contains(text(),'terms of use')]",
            androidLocator = null,
            page = this
    )

    val privacyPolicyLink = HybridPageElement(
            browserLocator = "//a[contains(text(),'privacy policy')]",
            androidLocator = null,
            page = this
    )

    val cookiesPolicyLink = HybridPageElement(
            browserLocator = "//a[contains(text(),'cookies policy')]",
            androidLocator = null,
            page = this
    )


    fun isMainErrorMessageVisible() : Boolean {
        return mainErrorMessage.element.isVisible
    }

    fun isMainBodyTextVisible() : Boolean {
        return mainBodyText.element.isVisible
    }

    fun isTermsOfUseLinkVisible() : Boolean {
        return termsOfUseLink.element.isVisible
    }

    fun isPrivacyPolicyLinkVisible() : Boolean {
        return privacyPolicyLink.element.isVisible
    }

    fun isCookiesPolicyLinkVisible() : Boolean {
        return cookiesPolicyLink.element.isVisible
    }

    fun isSecondaryErrorMessageVisible() : Boolean {
        return secondaryErrorMessage.element.isVisible
    }

    fun isTcCheckBoxVisible() : Boolean {
        return tcCheckBox.element.isVisible
    }

    fun isContinueButtonVisible() : Boolean {
        return continueButton.element.isVisible
    }

    fun clickTermsOfUseLink() {
        termsOfUseLink.element.click()
    }

    fun clickPrivacyPolicyLink() {
        privacyPolicyLink.element.click()
    }

    fun clickCookiesPolicyLink() {
        cookiesPolicyLink.element.click()
    }

    fun clickTcCheckbox() {
        tcCheckBox.element.click()
    }

    fun clickContinueButton() {
        continueButton.element.click()
    }

}
