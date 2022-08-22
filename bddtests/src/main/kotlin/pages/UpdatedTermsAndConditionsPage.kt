package pages

import net.thucydides.core.annotations.DefaultUrl
import pages.navigation.WebHeader
import pages.sharedElements.CheckBoxElement

@DefaultUrl("http://web.local.bitraft.io:3000/terms-and-conditions")
class UpdatedTermsAndConditionsPage : HybridPageObject() {
    private lateinit var webHeader: WebHeader
    private val pageHeaderText = "Updated conditions of use"

    val mainBodyText = HybridPageElement(
        webDesktopLocator = "//p",
        page = this
    ).withText("To continue using the NHS App, you need to agree to our updated", exact = false)

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
            text = "I understand and agree to the updated terms of use and privacy policy."
    )

    val continueButton = HybridPageElement(
        webDesktopLocator = "//button",
        page = this
    ).withNormalisedText("Continue")

    fun assertPageHeaderIsVisible() {
        webHeader.waitForPageHeaderText(pageHeaderText)
    }
}
