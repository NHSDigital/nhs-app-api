package pages

import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.openqa.selenium.WebElement
import pages.sharedElements.PublicHealthNotificationElement
import worker.models.serviceJourneyRules.PublicHealthNotification

@DefaultUrl("http://web.local.bitraft.io:3000/")
class HomePage : HybridPageObject() {

    val headerText: String = "Home"

    val userInfoDisplay = HybridPageElement(
        webDesktopLocator = "//section[@id='picture-banner-section']",
        page = this,
        helpfulName = "Picture Banner Info"
    )

    val userInfoDisplayProxy = HybridPageElement(
        webDesktopLocator = "//section[@id='proxy-picture-banner-section']",
        page = this,
        helpfulName = "Picture Banner Info"
    )

    private val nameInfoLargeScreen = HybridPageElement(
            webDesktopLocator = "//div[@data-sid='hero-user-name']",
            page = this,
            helpfulName = "Large Screen User Info"
    )

    private val nhsNumberInfoLargeScreen = HybridPageElement(
            webDesktopLocator = "//span[@data-sid='hero-user-nhs-number']",
            page = this,
            helpfulName = "Large Screen NHS Number"
    )

    private val unreadIndicator = HybridPageElement(
        webDesktopLocator = "//*[@id='btn_messages_discIndicator']",
        page = this,
        helpfulName = "Unread Indicator"
    )
        
    private val unreadCount = HybridPageElement(
        webDesktopLocator = "//*[@id='btn_messages_countIndicator']",
        page = this,
        helpfulName = "Unread Count"
    )

    private val surveyPath = "//div[@data-purpose='survey']"

    private val surveyLinkTab = HybridPageElement(
        webDesktopLocator = "$surveyPath/div[@data-purpose = 'tabForToggle']/div[@data-purpose = 'toggleContent']",
        page = this,
        helpfulName = "Survey Tab"
    )

    private val surveyContentPath = "$surveyPath/div[@data-purpose = 'content']"

    private val surveyContent = HybridPageElement(
        webDesktopLocator = surveyContentPath,
        page = this,
        helpfulName = "Survey Content"
    )

    val surveyContentLink = HybridPageElement(
        webDesktopLocator = "$surveyContentPath/p[@data-purpose = 'info']/a[@data-purpose = 'link']",
        page = this,
        helpfulName = "Survey Link"
    )

    private val listMenuPath = "//ul[@data-sid= 'navigation-list-menu']//a/div/h2"

    val banner = HybridPageElement(
        webDesktopLocator = "//*[@id='warning-banner']",
        page = this
    )

    val switchProfileLink = HybridPageElement(
        webDesktopLocator = "//*[@id='acting-as-other-user-switch']",
        page = this
    )

    private fun listOfLinks(): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = listMenuPath,
            page = this,
            helpfulName = "ListOfLinks"
        )
    }

    private fun link(linkText: String): HybridPageElement {
        return HybridPageElement(
            webDesktopLocator = "$listMenuPath${String.format(containsTextXpathSubstring, linkText)}",
            page = this,
            helpfulName = "$linkText Link")
    }

    fun assertUnreadIndicatorNotPresent() {
        unreadIndicator.assertElementNotPresent()
    }
    
    fun assertUnreadCountPresent() {
        unreadCount.assertIsVisible()
    }
    
    fun assertUnreadCountNotPresent() {
        unreadCount.assertElementNotPresent()
    }

    fun assertPatientNameIsVisible(requiredValue: String) {
        val displayedNameLarge = nameInfoLargeScreen.assertIsVisible().textValue
        assertEquals("Expected displayed to be $requiredValue", requiredValue, displayedNameLarge)
    }

    fun assertNhsNumberIsVisible(requiredValue: String) {
        val displayedName = nhsNumberInfoLargeScreen.assertIsVisible().textValue
        assertEquals("Expected displayed to be $requiredValue", requiredValue, displayedName)
    }

    fun assertNhsNumberIsNotPresent() {
        nhsNumberInfoLargeScreen.assertElementNotPresent()
    }

    fun assertHasProxyPatientDetails(expectedDetails: ArrayList<String>) {
        val actualDetails = arrayListOf<String>()
        userInfoDisplayProxy.actOnTheElement {
            actualDetails.addAll(
                    it.findElements<WebElement>(
                            By.xpath(
                                ".//div[@class='nhsuk-summary-list__row']"))
                            .map { element -> element.text })
        }
        assertCollection("ProxyPatientDetails", expectedDetails, actualDetails)
    }

    fun assertHasPublicHealthNotifications(publicHealthNotifications: List<PublicHealthNotification>) {
        publicHealthNotifications.forEach {
            PublicHealthNotificationElement(this, it).assertIsVisible()
        }
    }

    private fun assertCollection(message: String, expected: ArrayList<String>, actual: ArrayList<String>) {
        val listed = "Expected = ${expected.joinToString(", ")} Actual = ${actual.joinToString(", ")}"
        assertEquals("$message. Expected number. $listed", expected.count(), actual.count())
        Assert.assertTrue("$message. Contents. $listed", actual.containsAll(expected))
    }

    fun assertLinkIsVisible(linkText: String): HybridPageElement {
        return link(linkText).assertIsVisible()
    }

    fun assertLinkNotPresent(linkText: String) {
        link(linkText).assertElementNotPresent()
    }

    fun assertHomePageLinksNotPresent() {
        listOfLinks().assertElementNotPresent()
    }

    fun assertSurveyLinkCollapsibleAndExpandable() {
        surveyLinkTab.click()
        surveyContent.assertSingleElementPresent().assertIsNotVisible()
        surveyLinkTab.click()
        surveyLinkTab.assertSingleElementPresent().assertIsVisible()
    }

    fun assertSurveyLinkContent() {
        surveyLinkTab.assertSingleElementPresent().assertIsVisible()
        surveyContent.assertSingleElementPresent().assertIsVisible()
        assertEquals("Expected survey content",
                "Help us make this service better. Complete our quick survey.",
                surveyContent.text)
    }
}
