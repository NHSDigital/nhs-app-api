package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.navigation.HeaderNative
import pages.sharedElements.CheckBoxElement

@DefaultUrl("http://web.local.bitraft.io:3000/terms-and-conditions")
class TermsAndConditionsPage : HybridPageObject() {
    private lateinit var headerNative: HeaderNative
    private val pageHeaderText = "Accept conditions of use"

    val mainBodyText = HybridPageElement(
            webDesktopLocator = "//p",
            androidLocator = null,
            page = this
    ).withText("To use the NHS App you must agree to our", exact = false)

    val mainErrorMessage = HybridPageElement(
            webDesktopLocator = "//li",
            androidLocator = null,
            page = this
    ).withNormalisedText("You cannot continue without agreeing")

    val checkboxErrorMessage = HybridPageElement(
            webDesktopLocator = "//span",
            androidLocator = null,
            page = this
    ).withNormalisedText("You cannot use the NHS App without agreeing")

    val acceptTermsAndConditionsCheckBox = CheckBoxElement(
            page = this,
            text = "I understand and accept the terms of use and privacy policy."
    )

    val continueButton = HybridPageElement(
            webDesktopLocator = "//button",
            androidLocator = null,
            page = this
    ).withNormalisedText("Continue")

    fun assertPageHeaderIsVisible() {
        headerNative.waitForPageHeaderText(pageHeaderText)
    }
}
