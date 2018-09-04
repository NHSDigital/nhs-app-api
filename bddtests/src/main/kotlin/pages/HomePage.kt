package pages

import models.Patient
import net.serenitybdd.core.annotations.findby.By
import net.thucydides.core.annotations.DefaultUrl
import org.junit.Assert

@DefaultUrl("http://localhost:3000/")
open class HomePage : HybridPageObject(Companion.PageType.WEBVIEW_APP) {

    val headerText: String = "Home"

    private val greeting = HybridPageElement(
            browserLocator = "//h2[@data-purpose='greeting']",
            androidLocator = null,
            page = this,
            helpfulName = "Greeting"
    )

    private val betaBanner = HybridPageElement(
            browserLocator = "//div[div[contains(text(),'BETA')]][p[contains(text(),'This is a new service')]]",
            androidLocator = null,
            page = this,
            helpfulName = "Beta Banner"
    )

    private val surveyPath = "//div[@data-purpose='survey']"

    private val surveyLinkTab = HybridPageElement(
            browserLocator = "$surveyPath/div[@data-purpose = 'tabForToggle']/div[@data-purpose = 'toggleContent']",
            androidLocator = null,
            page = this,
            helpfulName = "Survey Tab"
    )

    private val surveyContentPath = "$surveyPath/div[@data-purpose = 'content']"

    private val surveyContent = HybridPageElement(
            browserLocator = surveyContentPath,
            androidLocator = null,
            page = this,
            helpfulName = "Survey Content"
    )

    val surveyContentLink = HybridPageElement(
            browserLocator = "$surveyContentPath/p[@data-purpose = 'info']/a[@data-purpose = 'link']",
            androidLocator = null,
            page = this,
            helpfulName = "Survey Link"
    )

    private val listMenuPath = "//div/ul/li/a"

    private fun listOfLinks(): HybridPageElement {
        return HybridPageElement(
                browserLocator = listMenuPath,
                androidLocator = null,
                page = this,
                helpfulName = "ListOfLinks"
        )
    }

    private fun link(linkText: String): HybridPageElement {
        return listOfLinks().containingText(linkText)
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
        return greeting.element.isCurrentlyVisible
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

    fun assertSurveyLinkCollapsibleAndExpandable() {
        surveyLinkTab.element.click()
        surveyContent.assertSingleElementPresent().assertIsNotVisible()
        surveyLinkTab.element.click()
        surveyLinkTab.assertSingleElementPresent().assertIsVisible()
    }

    fun assertSurveyLinkContent() {
        surveyLinkTab.assertSingleElementPresent().assertIsVisible()
        surveyContent.assertSingleElementPresent().assertIsVisible()
        Assert.assertEquals("Expected survey content",
                "Help us make this service better. Complete our quick survey.",
                surveyContent.element.text)
    }
}
