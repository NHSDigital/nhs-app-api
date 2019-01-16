package pages

import models.Patient
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert

@DefaultUrl("http://web.local.bitraft.io:3000/")
open class HomePage : HybridPageObject() {

    val headerText: String = "Home"

    val greeting = HybridPageElement(
            webDesktopLocator = "//h2[@data-purpose='greeting']",
            androidLocator = null,
            page = this,
            helpfulName = "Greeting"
    )

    private val betaBanner = HybridPageElement(
            webDesktopLocator = "//div[span[@data-purpose = 'beta-banner']]//div[contains(text(), " +
                    "'BETA')]/following-sibling::p[contains(text(),'This is a new service')]",
            androidLocator = null,
            page = this,
            helpfulName = "Beta Banner"
    )

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

    private val listMenuPath = "//ul[@data-sid= 'navigation-list-menu']//a"


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

    val checkSymptomsLink = link("Check my symptoms")

    val bookAndManageAppointmentsLink = link("Book and manage appointments")

    val orderRepeatPrescriptionLink = link("Order a repeat prescription")

    val viewMedicalRecordLink = link("View my medical record")

    val organDonationLink = link("Set organ donation preferences")

    val expectedLinks = arrayListOf(checkSymptomsLink,
            bookAndManageAppointmentsLink,
            orderRepeatPrescriptionLink,
            viewMedicalRecordLink,
            organDonationLink
    )

    fun assertHasWelcomeMessageFor(patient: Patient) {
        val name = "${patient.title} ${patient.firstName} ${patient.surname}".trim()
        val expected = "Welcome, $name"
        val text = greeting.element.text
        Assert.assertEquals("Welcome message did not match", expected, text)
    }

    fun assertHasPatientDetails(patient: Patient, expectedDetails: ArrayList<String>) {

        assertHasWelcomeMessageFor(patient)
        val actualDetails = arrayListOf<String>()
        actualDetails.addAll(
                greeting.element.findElements(
                        By.xpath("./following-sibling::div[1]/p"))
                        .map { element -> element.text })
        assertCollection("PatientDetails", expectedDetails, actualDetails)
    }

    fun isWelcomeHeaderVisible(): Boolean {
        return greeting.element.isVisible
    }

    private fun assertCollection(message: String, expected: ArrayList<String>, actual: ArrayList<String>) {
        val listed = "Expected = ${expected.joinToString(", ")} Actual = ${actual.joinToString(", ")}"
        Assert.assertEquals("$message. Expected number. $listed", expected.count(), actual.count())
        Assert.assertTrue("$message. Contents. $listed", actual.containsAll(expected))
    }

    fun assertLinksPresentWithinHomePageBody() {
        val links = listOfLinks().elements
        Assert.assertEquals("Number of expected Links",
                expectedLinks.count(),
                links.count())
        expectedLinks.forEach { link -> link.assertSingleElementPresent() }
    }

    fun assertBetaBannerVisible() {
        betaBanner.assertSingleElementPresent()
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
                surveyContent.element.text)
    }

    fun assertNativeElementsLoaded(){
        if(onMobile()) {
            shouldBeVisibleOnNative(greeting)
        }
    }
}
