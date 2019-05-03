package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/terms-and-conditions")
class UpdatedTermsAndConditionsPage : HybridPageObject() {

    val updatedTermsAndConditionsLabel = HybridPageElement(
            webDesktopLocator = "//label[@id='termsAndConditionsCheckboxLabel']",
            webMobileLocator  = "//label[@id='termsAndConditionsCheckboxLabel']",
            androidLocator = null,
            page = this
    )

    val continueButton = HybridPageElement(
            webDesktopLocator  = "//*[@id='btn_accept']",
            webMobileLocator = "//*[@id='btn_accept']",
            androidLocator = null,
            page = this
    )

    val mainErrorMessage = HybridPageElement(
            webDesktopLocator = "//*[@id='error_msg']",
            webMobileLocator = "//*[@id='error_msg']",
            androidLocator = null,
            page = this
    )

    val secondaryErrorMessage = HybridPageElement(
            webDesktopLocator = "//*[@id='error_txt']",
            webMobileLocator = "//*[@id='error_txt']",
            androidLocator = null,
            page = this
    )

    fun isSecondaryErrorMessageVisible() : Boolean {
        return secondaryErrorMessage.isVisible
    }

    fun isMainErrorMessageVisible() : Boolean {
        return mainErrorMessage.isVisible
    }
}
