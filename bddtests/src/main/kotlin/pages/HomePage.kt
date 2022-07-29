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

    val welcomeInfo = HybridPageElement(
        webDesktopLocator = "//div[@data-sid='welcome-info']",
        page = this,
        helpfulName = "Welcome info"
    )

    val welcomeInfoProxy = HybridPageElement(
        webDesktopLocator = "//div[@data-sid='welcome-info-proxy']",
        page = this,
        helpfulName = "Welcome info"
    )

    val dismissButton = HybridPageElement(
        webDesktopLocator = "//a[@id='btn_biometricBannerDismiss']",
        page = this,
        helpfulName = "Dismiss Button"
    ).withText("Dismiss", false)

    private val unreadIndicator = HybridPageElement(
        webDesktopLocator = "//*[@id='btn_messages_discIndicator']",
        page = this,
        helpfulName = "Unread Indicator")
        
    private val unreadCount = HybridPageElement(
        webDesktopLocator = "//*[@id='btn_messages_countIndicator']",
        page = this,
        helpfulName = "Unread Count")

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

    val actingAsOtherUserWarning = HybridPageElement(
        webDesktopLocator = "//*[@id='acting-as-other-user-warning']",
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

    fun assertPatientDetailIsVisible(detail:String, value: String) {
        val element = getPatientDetailElement(detail).assertIsVisible()
        assertEquals("Expected $detail to be $value", value, element.text)
    }

    fun assertPatientDetailIsNotPresent(detail:String) {
        getPatientDetailElement(detail).assertElementNotPresent()
    }

    private fun getPatientDetailElement(detail:String) : HybridPageElement {
        return HybridPageElement(
            webDesktopLocator =
              "${welcomeInfo.webDesktopLocator}//dt[normalize-space(text())='$detail:']/following-sibling::dd[1]",
            page = this
        )
    }

    fun assertHasProxyPatientDetails(expectedDetails: ArrayList<String>) {
        val actualDetails = arrayListOf<String>()
        welcomeInfoProxy.actOnTheElement {
            actualDetails.addAll(
                    it.findElements<WebElement>(
                            By.xpath("//div[@class='nhsuk-summary-list__row']"))
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
