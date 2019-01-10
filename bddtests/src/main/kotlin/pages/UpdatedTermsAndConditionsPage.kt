package pages

import net.thucydides.core.annotations.DefaultUrl

@DefaultUrl("http://web.local.bitraft.io:3000/terms-and-conditions")
class UpdatedTermsAndConditionsPage : HybridPageObject() {

    val updatedTermsAndConditionsLabel = HybridPageElement(
            browserLocator = "//label[@id='termsAndConditionsCheckboxLabel']",
            androidLocator = null,
            page = this
    )

    val continueButton = HybridPageElement(
            browserLocator = "//*[@id='btn_accept']",
            androidLocator = null,
            page = this
    )

    val mainErrorMessage = HybridPageElement(
            browserLocator = "//*[@id='error_msg']",
            androidLocator = null,
            page = this
    )

    val secondaryErrorMessage = HybridPageElement(
            browserLocator = "//*[@id='error_txt']",
            androidLocator = null,
            page = this
    )

    fun isSecondaryErrorMessageVisible() : Boolean {
        return secondaryErrorMessage.element.isVisible
    }

    fun isMainErrorMessageVisible() : Boolean {
        return mainErrorMessage.element.isVisible
    }
}
