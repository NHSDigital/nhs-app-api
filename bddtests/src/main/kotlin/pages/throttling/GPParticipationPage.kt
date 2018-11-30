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
        browserLocator = "//h2[contains(text(), 'Features used by ')]",
        androidLocator = null,
        page = this
    )

    val featuresUsedHeaderParticipatingPractice = HybridPageElement(
        browserLocator = "//h2[contains(text(), 'Features used by $ORGANISATION_NAME 1')]",
        androidLocator = null,
        page = this
    )

    val featuresUsedHeaderNotParticipatingPractice = HybridPageElement(
        browserLocator = "//h2[contains(text(), 'Features used by $ORGANISATION_NAME 2')]",
        androidLocator = null,
        page = this
    )

    val currentlyAvailableHeader = HybridPageElement(
        browserLocator = "//h2[contains(text(), 'Currently available')]",
        androidLocator = null,
        page = this
    )

    val comingSoonHeader = HybridPageElement(
        browserLocator = "//h2[contains(text(), 'Coming soon')]",
        androidLocator = null,
        page = this
    )

    val availableFeatures = HybridPageElement(
        browserLocator = "//ul[@id='availableFeatures']/li",
        androidLocator = null,
        page = this
    )

    val unavailableFeatures = HybridPageElement(
        browserLocator = "//ul[@id='unavailableFeatures']/li",
        androidLocator = null,
        page = this
    )

    val ctaContinueButton = HybridPageElement(
        browserLocator = "//a[contains(text(), 'Continue')]",
        androidLocator = null,
        page = this
    )

    val ctaCreateAccountButton = HybridPageElement(
            browserLocator = "//button[contains(text(), 'Create account')]",
            androidLocator = null,
            page = this
    )

    val ctaNotMySurgeryButton = HybridPageElement(
            browserLocator = "//a[contains(text(), 'This is not my GP surgery')]",
            androidLocator = null,
            page = this
    )

    val createAccountMessage = HybridPageElement(
        browserLocator = "//p[@id='createAccountMessage']",
        androidLocator = null,
        page = this
    )

    val limitingFeaturesWarning = HybridPageElement(
        browserLocator = "//p[@id='limitingFeaturesWarning']",
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