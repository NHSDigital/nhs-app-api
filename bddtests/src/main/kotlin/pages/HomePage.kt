package pages

import mockingFacade.linkedProfiles.LinkedProfileFacade
import models.Patient
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert
import org.junit.Assert.assertEquals
import org.openqa.selenium.WebElement
import pages.sharedElements.PublicHealthNotificationElement
import utils.SerenityHelpers
import worker.models.serviceJourneyRules.PublicHealthNotification

@DefaultUrl("http://web.local.bitraft.io:3000/")
open class HomePage : HybridPageObject() {

    val headerText: String = "Home"

    val greeting = HybridPageElement(
            webDesktopLocator = "//h2[@data-purpose='greeting']",
            androidLocator = null,
            page = this,
            helpfulName = "Greeting"
    )

    val dismissButton = HybridPageElement(
            webDesktopLocator = "//a[@id='btn_biometricBannerDismiss']",
            androidLocator = null,
            page = this,
            helpfulName = "Dismiss Button"
    ).withText("Dismiss", false)

    private val surveyPath = "//div[@data-purpose='survey']"

    private val surveyLinkTab = HybridPageElement(
            webDesktopLocator = "$surveyPath/div[@data-purpose = 'tabForToggle']/div[@data-purpose = 'toggleContent']",
            androidLocator = null,
            page = this,
            helpfulName = "Survey Tab"
    )

    private val surveyContentPath = "$surveyPath/div[@data-purpose = 'content']"

    private val surveyContent = HybridPageElement(
            webDesktopLocator = surveyContentPath,
            androidLocator = null,
            page = this,
            helpfulName = "Survey Content"
    )

    val surveyContentLink = HybridPageElement(
            webDesktopLocator = "$surveyContentPath/p[@data-purpose = 'info']/a[@data-purpose = 'link']",
            androidLocator = null,
            page = this,
            helpfulName = "Survey Link"
    )

    private val listMenuPath = "//ul[@data-sid= 'navigation-list-menu']//a/span/div/h2"

    val banner = HybridPageElement(
            webDesktopLocator = "//*[@id='yellow-banner']",
            webMobileLocator = "//*[@id='yellow-banner']",
            page = this
    )

    val actingAsOtherUserWarning = HybridPageElement(
            webDesktopLocator = "//*[@id='acting-as-other-user-warning']",
            webMobileLocator = "//*[@id='acting-as-other-user-warning']",
            page = this
    )

    private fun listOfLinks(): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = listMenuPath,
                androidLocator = null,
                page = this,
                helpfulName = "ListOfLinks"
        )
    }

    private fun link(linkText: String): HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "$listMenuPath${String.format(containsTextXpathSubstring, linkText)}",
                androidLocator = null,
                page = this,
                helpfulName = "$linkText Link")
    }

    val checkSymptomsLink = link("Check your symptoms")

    val bookAndManageAppointmentsLink = link("Book and manage appointments")

    val orderRepeatPrescriptionLink = link("Order a repeat prescription")

    val viewMedicalRecordLink = link("View your GP medical record")

    val organDonationLink = link("Manage your organ donation decision")

    val linkedProfilesLink = link("Linked profiles")

    val messagesLink = link("View your messages")

    val expectedLinks = arrayListOf(checkSymptomsLink,
            bookAndManageAppointmentsLink,
            orderRepeatPrescriptionLink,
            viewMedicalRecordLink,
            messagesLink,
            organDonationLink
    )

    val expectedLinksIncludingLinkedProfiles = expectedLinks.union(listOf(linkedProfilesLink))

    fun assertHasWelcomeMessageFor(patient: Patient) {
        val name = patient.formattedFullName()
        val expected = "Welcome, $name"
        val text = greeting.text
        assertEquals("Welcome message did not match", expected, text)
    }

    fun assertPatientDetailIsVisible(detail:String, value: String) {
        val element = getPatientDetailElement(detail)
        element.assertIsVisible()
        assertEquals("Expected $detail to be $value", value, element.text)
    }

    fun assertPatientDetailIsNotPresent(detail:String) {
        getPatientDetailElement(detail).assertElementNotPresent()
    }

    private fun getPatientDetailElement(detail:String) : HybridPageElement {
        return HybridPageElement(
                webDesktopLocator = "${greeting.webDesktopLocator}/following-sibling::div[1]/" +
                        "p[strong[text()='$detail:']]/span",
                androidLocator = null,
                page = this
        )
    }

    fun assertHasProxyPatientDetails(proxyPatient: LinkedProfileFacade, expectedDetails: ArrayList<String>) {

        assertHasWelcomeMessageFor(proxyPatient.profile)
        val actualDetails = arrayListOf<String>()
        greeting.actOnTheElement {
            actualDetails.addAll(
                    it.findElements<WebElement>(
                            By.xpath("./following-sibling::div[1]/p"))
                            .map { element -> element.text })
        }
        assertCollection("ProxyPatientDetails", expectedDetails, actualDetails)
    }

    fun assertHasPublicHealthNotifications(publicHealthNotifications: List<PublicHealthNotification>) {
        publicHealthNotifications.forEach {
            PublicHealthNotificationElement(this, it).assertIsVisible()
        }
    }

    fun isWelcomeHeaderVisible(): Boolean {
        return greeting.isVisible
    }

    private fun assertCollection(message: String, expected: ArrayList<String>, actual: ArrayList<String>) {
        val listed = "Expected = ${expected.joinToString(", ")} Actual = ${actual.joinToString(", ")}"
        Assert.assertEquals("$message. Expected number. $listed", expected.count(), actual.count())
        Assert.assertTrue("$message. Contents. $listed", actual.containsAll(expected))
    }

    fun assertLinksPresentWithinHomePageBody() {
        val links = listOfLinks().elements

        val patient = SerenityHelpers.getPatient()
        val linksToCheck = if (patient.linkedAccounts.count() > 0) {
            expectedLinksIncludingLinkedProfiles
        }
        else {
            expectedLinks
        }

        linksToCheck.forEach { link -> link.assertSingleElementPresent() }
        Assert.assertEquals("Number of expected Links",
                linksToCheck.count(),
                links.count())
    }

    fun assertHomePageLinksNotPresent() {
        listOfLinks().assertElementNotPresent()
    }

    fun assertVersionNumberVisible() {

        val divXpath = "//div[contains(text(),'Version dev_bdd_docker')]"
        val pXpath = "//p[contains(text(),'Version dev_bdd_docker')]"

        val versionNumberElement = HybridPageElement(
                webDesktopLocator = "$divXpath | $pXpath",
                androidLocator = null,
                page = this,
                helpfulName = "Version Number"
        )
        versionNumberElement.assertSingleElementPresent()
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
        Assert.assertEquals("Expected survey content",
                "Help us make this service better. Complete our quick survey.",
                surveyContent.text)
    }
}
