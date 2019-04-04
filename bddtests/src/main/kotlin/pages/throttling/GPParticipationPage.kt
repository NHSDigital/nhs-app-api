package pages.throttling

import mocking.data.nhsAzureSearchData.NhsAzureSearchData.ORGANISATION_NAME
import pages.HybridPageElement
import pages.HybridPageObject
import pages.sharedElements.TextBlockElement

class GPParticipationPage : HybridPageObject() {

    val featuresUsedHeaderParticipatingPractice = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'NHS App features used by $ORGANISATION_NAME 1')]",
            webMobileLocator = "//h2[contains(text(), 'NHS App features used by $ORGANISATION_NAME 1')]",
            androidLocator = null,
            page = this
    )

    val featuresUsedHeaderNotParticipatingPractice = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'NHS App features used by $ORGANISATION_NAME 2')]",
            webMobileLocator = "//h2[contains(text(), 'NHS App features used by $ORGANISATION_NAME 2')]",
            androidLocator = null,
            page = this
    )

    val notMySurgeryLink = HybridPageElement(
            webDesktopLocator = "//a[contains(text(), 'This is not my GP surgery')]",
            webMobileLocator = "//a[contains(text(), 'This is not my GP surgery')]",
            androidLocator = null,
            page = this
    )

    val createAccountMessage = HybridPageElement(
            webDesktopLocator = "//p[@id='createAccountMessage']",
            webMobileLocator = "//p[@id='createAccountMessage']",
            androidLocator = null,
            page = this
    )

    val limitingFeaturesWarning = HybridPageElement(
            webDesktopLocator = "//p[@id='limitingFeaturesWarning']",
            webMobileLocator = "//p[@id='limitingFeaturesWarning']",
            androidLocator = null,
            page = this
    )

    fun assertNotParticipatingFeaturesVisible() {
        TextBlockElement.withH2Header("Currently available", this)
                .assertList(
                        "Check symptoms",
                        "Record organ donation decision")

        TextBlockElement.withH2Header("Coming soon", this)
                .assertList(
                        "Book and manage appointments",
                        "Order repeat prescriptions",
                        "View your medical records")
    }

    fun assertParticipatingFeaturesVisible() {
        TextBlockElement.withH2Header("Currently available", this)
                .assertList(
                        "Check symptoms",
                        "Book and manage appointments",
                        "Order repeat prescriptions",
                        "View your medical records")

        TextBlockElement.withH2Header("Coming soon", this)
                .assertElementNotPresent()
    }
}