package pages.linkedProfiles.shutterPages

import pages.HybridPageElement
import pages.HybridPageObject

abstract class ShutterComponent : HybridPageObject() {

    val subHeaderText = HybridPageElement(
            webDesktopLocator = "//p[@id='shutter-subheader-text']",
            page = this
    )

    val summaryText = HybridPageElement(
            webDesktopLocator = "//p[@id='shutter-summary-text']",
            page = this
    )

    val switchText = HybridPageElement(
            webDesktopLocator = "//p[@id='shutter-switch-text']",
            page = this
    )

    val switchProfileButton = HybridPageElement(
            webDesktopLocator = "//button[@id='btn-switch-profile']",
            page = this
    ).withText("Switch to my profile", false)
}
