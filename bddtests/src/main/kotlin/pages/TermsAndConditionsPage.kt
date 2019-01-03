package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/terms-and-conditions")
class TermsAndConditionsPage : HybridPageObject() {

    val mainErrorMessage = HybridPageElement(
            webDesktopLocator = "//*[@id='error_msg']",
            androidLocator = null,
            page = this
    )

    val mainBodyText = HybridPageElement(
            webDesktopLocator = "//*[@id='text_body']",
            androidLocator = null,
            page = this
    )

    val secondaryErrorMessage = HybridPageElement(
            webDesktopLocator = "//*[@id='error_txt']",
            androidLocator = null,
            page = this
    )

    val tcCheckBox = HybridPageElement(
            webDesktopLocator = "//input[@id='termsAndConditions-agree_checkbox']",
            androidLocator = null,
            page = this
    )

    val termsAndConditionsLabel = HybridPageElement(
            webDesktopLocator = "//label[@id='termsAndConditionsCheckboxLabel']",
            androidLocator = null,
            page = this
    )

    val continueButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_accept']",
            androidLocator = null,
            page = this
    )

    val termsOfUseLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(),'terms of use')]",
            androidLocator = null,
            page = this
    )

    val privacyPolicyLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(),'privacy policy')]",
            androidLocator = null,
            page = this
    )

    val cookiesPolicyLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(),'cookies policy')]",
            androidLocator = null,
            page = this
    )

    val tcBackButton = HybridPageElement(
            webDesktopLocator = "//*[@id='backIcon']",
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
}
