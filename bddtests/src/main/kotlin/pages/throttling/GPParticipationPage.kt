package pages.throttling

import mocking.data.nhsAzureSearchData.NhsAzureSearchData.ORGANISATION_NAME
import org.junit.Assert.assertTrue
import pages.HybridPageObject
import pages.HybridPageElement

private const val NUM_PARTICIPATING_AVAILABLE_FEATURES = 4
private const val NUM_PARTICIPATING_UNAVAILABLE_FEATURES = 0
private const val NUM_NOT_PARTICIPATING_AVAILABLE_FEATURES = 1
private const val NUM_NOT_PARTICIPATING_UNAVAILABLE_FEATURES = 3

class GPParticipationPage : HybridPageObject() {

    var featuresUsedHeader = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'NHS App features used by ')]",
            webMobileLocator = "//h2[contains(text(), 'NHS App features used by ')]",
            androidLocator = null,
            page = this
    )

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

    val currentlyAvailableHeader = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'Currently available')]",
            webMobileLocator = "//h2[contains(text(), 'Currently available')]",
            androidLocator = null,
            page = this
    )

    val comingSoonHeader = HybridPageElement(
            webDesktopLocator = "//h2[contains(text(), 'Coming soon')]",
            webMobileLocator = "//h2[contains(text(), 'Coming soon')]",
            androidLocator = null,
            page = this
    )

    val availableFeatures = HybridPageElement(
            webDesktopLocator = "//ul[@id='availableFeatures']/li",
            webMobileLocator = "//ul[@id='availableFeatures']/li",
            androidLocator = null,
            page = this
    )

    val unavailableFeatures = HybridPageElement(
            webDesktopLocator = "//ul[@id='unavailableFeatures']/li",
            webMobileLocator = "//ul[@id='unavailableFeatures']/li",
            androidLocator = null,
            page = this
    )

    val ctaParticipatingContinueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            webMobileLocator = "//button[contains(text(), 'Continue')]",
            androidLocator = null,
            page = this
    )

    val ctaNotParticipatingContinueButton = HybridPageElement(
            webDesktopLocator = "//button[contains(text(), 'Continue')]",
            webMobileLocator = "//button[contains(text(), 'Continue')]",
            androidLocator = null,
            page = this
    )

    val ctaNotMySurgeryButton = HybridPageElement(
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
        assertTrue(unavailableFeatures.elements.count() == NUM_NOT_PARTICIPATING_UNAVAILABLE_FEATURES)
        assertTrue(availableFeatures.elements.count() == NUM_NOT_PARTICIPATING_AVAILABLE_FEATURES)
    }

    fun assertParticipatingFeaturesVisible() {
        assertTrue(unavailableFeatures.elements.count() == NUM_PARTICIPATING_UNAVAILABLE_FEATURES)
        assertTrue(availableFeatures.elements.count() == NUM_PARTICIPATING_AVAILABLE_FEATURES)
    }

    fun clickNotMySurgeryButton() {
        ctaNotMySurgeryButton.click()
    }

    fun setHeaderToLookFor(participating: Boolean) {
        featuresUsedHeader = if (participating) {
            featuresUsedHeaderParticipatingPractice
        } else {
            featuresUsedHeaderNotParticipatingPractice
        }
    }
}