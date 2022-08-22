package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.navigation.WebHeader
import pages.sharedElements.CheckBoxElement

@DefaultUrl("http://web.local.bitraft.io:3000/terms-and-conditions")
class TermsAndConditionsPage : HybridPageObject() {
    private lateinit var webHeader: WebHeader
    private val pageHeaderText = "Accept conditions of use"

    val mainBodyText = HybridPageElement(
        webDesktopLocator = "//p",
        page = this
    ).withText("To use your NHS account you must agree to our", exact = false)

    val mainErrorMessage = HybridPageElement(
        webDesktopLocator = "//a",
        page = this
    ).withNormalisedText("Confirm if you agree with the terms of use and necessary cookies")

    val checkboxErrorMessage = HybridPageElement(
        webDesktopLocator = "//span",
        page = this
    ).withNormalisedText("Confirm if you agree with the terms of use and necessary cookies")

    val acceptTermsAndConditionsCheckBox = CheckBoxElement(
            page = this,
            text = "I understand and accept the terms of use and privacy policy."
    )

    val acceptCookiesCheckBox = CheckBoxElement(
            page = this,
            text = "I accept the use of optional analytic cookies used to improve performance."
    )

    val continueButton = HybridPageElement(
        webDesktopLocator = "//button",
        page = this
    ).withNormalisedText("Continue")

    fun assertPageHeaderIsVisible() {
        webHeader.waitForPageHeaderText(pageHeaderText)
    }
}
