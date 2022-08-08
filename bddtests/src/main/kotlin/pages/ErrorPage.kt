package pages

import org.junit.Assert.assertEquals
import pages.navigation.WebHeader

open class ErrorPage : HybridPageObject() {
    private val errorTextFinderFormat = "//div[@data-purpose='error']/p[@data-purpose='%s']"

    private val headerLocator = String.format(errorTextFinderFormat, "msg-header")
    private val subHeaderLocator = String.format(errorTextFinderFormat, "msg-subheader")
    private val messageTextLocator = String.format(errorTextFinderFormat, "msg-text")
    private val ifYouNeedToBookAnAppointmentTextLocator =
        String.format(errorTextFinderFormat, "ifYouNeedToBookAnAppointment-text")
    private val backLinkLocator = "//*[@data-purpose='back-button']"
    private val backToPrescriptionsLinkLocator = "//*[@id='prescriptionsLink']"
    private val desktopBackButtonLocator = "//*[@data-purpose='retry-or-back-button']"
    private val desktopBackLinkLocator = "//*[@id='backLink']"
    private val desktopDeviceSettingsLinkLocator = "//*[@id='device-settings']"

    lateinit var webHeader: WebHeader

    val heading = findElementByLocator(headerLocator)

    val subHeading = findElementByLocator(subHeaderLocator)

    val errorText1 = findElementByLocator(messageTextLocator)

    val errorIfYouNeedToBookAnAppointment = findElementByLocator(ifYouNeedToBookAnAppointmentTextLocator)

    val button = findElementByLocator(desktopBackButtonLocator)

    val link = findElementByLocator(desktopBackLinkLocator)

    val customBackLink = findElementByLocator(backLinkLocator)

    val backToPrescriptionsLink = findElementByLocator(backToPrescriptionsLinkLocator)

    val deviceSettings = findElementByLocator(desktopDeviceSettingsLinkLocator)

    protected fun findElementByLocator(locator: String): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = locator,
            page = this
        )
    }

    private val headerWeb = HybridPageElement(
        webDesktopLocator = "//p",
        page = this
    ).withNormalisedText("Service currently unavailable")

    fun assertNoButton(text: String) {
        button.withText(text).assertElementNotPresent()
    }

    fun clickButton() {
        button.click()
    }

    fun assertHasButton(expectedText: String) {
        button.assertSingleElementPresent()
        assertEquals("Expected button text", expectedText, button.text)
    }

    override fun assertPageHeader(headerText: String): ErrorPage {
        super.assertPageHeader(headerText)
        return this
    }

    fun assertHeaderText(headerText: String): ErrorPage {
        assertEquals("Content header incorrect. ", headerText, heading.text)
        return this
    }

    fun assertBackToPrescriptionsLinkText(linkText: String): ErrorPage {
        assertEquals("Link Text incorrect. ", linkText, backToPrescriptionsLink.text)
        return this
    }

    fun assertNoSubHeader(): ErrorPage {
        subHeading.assertElementNotPresent()
        return this
    }

    fun assertSubHeaderText(subHeaderText: String): ErrorPage {
        assertEquals("Content sub-header incorrect. ", subHeaderText, subHeading.text)
        return this
    }

    fun assertMessageText(messageText: String): ErrorPage {
        assertEquals("Content message incorrect. ", messageText, errorText1.text)
        return this
    }

    fun assertIfYouNeedToBookAnAppointmentText(messageText: String): ErrorPage {
        assertEquals("Content message incorrect. ", messageText, errorIfYouNeedToBookAnAppointment.text)
        return this
    }

    fun assertRetryButtonText(retryButtonText: String): ErrorPage {
        assertEquals("Retry button text incorrect. ", retryButtonText, button.text)
        return this
    }

    fun assertDeviceSettingsText(deviceSettingsText: String): ErrorPage {
        assertEquals("Device settings text incorrect. ", deviceSettingsText, deviceSettings.text)
        return this
    }

    fun assertNoRetryButton(): ErrorPage {
        button.assertElementNotPresent()
        return this
    }

    fun assertIsNotDisplayed() {
        headerWeb.assertElementNotPresent()
    }

    fun assertLinkText(linkText: String): ErrorPage {
        assertEquals("Link text incorrect. ", linkText, link.text)
        return this
    }
}
