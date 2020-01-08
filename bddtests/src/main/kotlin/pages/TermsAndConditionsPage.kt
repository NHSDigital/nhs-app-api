package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.sharedElements.CheckBoxElement

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

    private val tcCheckBox = CheckBoxElement(
            page = this,
            text = "I understand and accept the terms of use and privacy policy."
    )

    val termsAndConditionsLabel = HybridPageElement(
            webDesktopLocator = "//label[@id='termsAndConditions-agree_checkbox-label']",
            androidLocator = null,
            page = this
    )

    val continueButton = HybridPageElement(
            webDesktopLocator = "//*[@id='btn_accept']",
            androidLocator = null,
            page = this
    )

    fun isMainErrorMessageVisible() : Boolean {
        return mainErrorMessage.isVisible
    }

    fun isMainBodyTextVisible() : Boolean {
        return mainBodyText.isVisible
    }

    fun isSecondaryErrorMessageVisible() : Boolean {
        return secondaryErrorMessage.isVisible
    }

    fun assertTcCheckBoxVisible() {
        tcCheckBox.assertIsVisible()
    }

    fun isContinueButtonVisible() : Boolean {
        return continueButton.isVisible
    }
}
