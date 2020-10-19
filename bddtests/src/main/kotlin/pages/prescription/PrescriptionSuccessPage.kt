package pages.prescription

import mocking.nhsAzureSearchService.NhsAzureSearchOrganisationItem
import models.prescriptions.MedicationCourse
import net.serenitybdd.core.pages.WebElementFacade
import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.asciiText
import pages.assertElementNotPresent
import pages.text
import pages.navigation.HeaderNative

class PrescriptionSuccessPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    private val whatHappensNextHeading = HybridPageElement(
        webDesktopLocator = "//h2[contains(text(),'What happens next')]",
        page = this
    )

    private val orderSummaryPreText = HybridPageElement(
        webDesktopLocator = "//p[@data-purpose='pre-text']",
        page = this
    )

    private val orderSummaryCard = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='order-success-summary']",
        page = this
    )

    private val prescriptionHasBeenSentText = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='what-happens-next']/p",
        page = this
    )

    private val noNominatedPharmacyWhatHappensNextText = HybridPageElement(
        webDesktopLocator = "//p[@data-purpose='what-happens-next-no-nom-pharm']",
        page = this
    )

    private val highStreetNomPharmWhatHappensNextText = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='what-happens-next-high-street-pharm']/p",
        page = this
    )

    private val internetPharmWhatHappensNext = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='what-happens-next-online-only-pharm']",
        page = this
    )

    private val defaultWhatHappensNextText = HybridPageElement(
        webDesktopLocator = "//p[@data-purpose='what-happens-next-default']",
        page = this
    )

    private val proxyWhatHappensNextText = HybridPageElement(
        webDesktopLocator = "//div[@data-purpose='what-happens-next-proxy']/p",
        page = this
    )

    private val goToViewOrdersLink = HybridPageElement(
        webDesktopLocator = "//a[contains(text(),\"Go to your prescription orders\")]",
        page = this
    )

    fun checkSuccessHeading() {
        headerNative.waitForPageHeaderText("Your prescription has been ordered")
    }

    fun checkSuccessHeadingForProxy(name: String) {
        headerNative.waitForPageHeaderText("$name\'s prescription has been ordered")
    }

    fun clickGoToViewOrdersLink() {
        goToViewOrdersLink.click()
    }

    fun checkWhatHappensNextNoNominatedPharmacy() {
        checkOrderSummaryPreText("You have ordered:")
        checkWhatHappensNextHeading()
        checkPrescriptionHasBeenSentText()
        Assert.assertEquals("Advice for no nominated pharmacy is incorrect",
            "Once they approve your prescription, you'll need to collect it from your GP surgery.",
                noNominatedPharmacyWhatHappensNextText.text)
    }

    fun checkWhatHappensNextHighStreetPharmacy(pharmacyName: String) {
        checkOrderSummaryPreText("You have ordered:")
        checkWhatHappensNextHeading()
        checkPrescriptionHasBeenSentText()
        checkSendToNominatedPharmacyText(pharmacyName, highStreetNomPharmWhatHappensNextText.text)
    }

    fun checkWhatHappensNextInternetPharmacy(pharmacy: NhsAzureSearchOrganisationItem) {
        val name = pharmacy.OrganisationName
        val telephone = pharmacy.primaryPhone()

        val paragraphs = internetPharmWhatHappensNext.elements.first().thenFindAll("p")

        checkOrderSummaryPreText("You have ordered:")
        checkWhatHappensNextHeading()
        checkPrescriptionHasBeenSentText()
        checkSendToNominatedPharmacyText(name, paragraphs[0].text)
        Assert.assertEquals("Register advice for internet pharmacy incorrect",
            "You may need to register with $name separately at ${pharmacy.URL} or call them at $telephone.",
            paragraphs[1].text)
        checkOnlinePharmacyOncePreparedText(name, paragraphs[2].text)
    }

    fun checkWhatHappensNext() {
        checkOrderSummaryPreText("You have ordered:")
        checkWhatHappensNextHeading()
        checkPrescriptionHasBeenSentText()
        Assert.assertEquals("Default advice for multiple pharmacy types/dispensing practice/EPS disabled is incorrect",
            "The order status will be updated once it has been reviewed by your GP.",
            defaultWhatHappensNextText.text)
    }

    fun checkWhatHappensNextForProxy(name: String) {
        checkOrderSummaryPreText("You have ordered a prescription on behalf of $name.")
        checkWhatHappensNextHeading()
        Assert.assertEquals("What happens next content incorrect for proxy",
            "The order status will be updated once it has been reviewed by $name's GP.",
            proxyWhatHappensNextText.text)
    }

    fun checkHasNoOrderSummary() {
        orderSummaryCard.assertElementNotPresent()
    }

    fun checkHasOrderSummary(selectedCourses: List<MedicationCourse>) {
        val prescriptions =
            orderSummaryCard.elements.first().thenFindAll("//div[@data-purpose='prescription-summary']")

        Assert.assertEquals(selectedCourses.size, prescriptions.size)

        for (i in selectedCourses.indices) {
            val expectedCourse = selectedCourses[i]
            val currentCourseOnScreen = prescriptions[i]

            val nameOnScreen = currentCourseOnScreen.findBy<WebElementFacade>(
                "[data-purpose='prescription-name']")
            val instructionsOnScreen = currentCourseOnScreen.findBy<WebElementFacade>(
                "[data-purpose='prescription-details']").asciiText

            Assert.assertEquals("Prescription course name incorrect",
                expectedCourse.name,
                nameOnScreen.text)
            Assert.assertEquals("Prescription course description incorrect",
                expectedCourse.getInstructionsText(),
                instructionsOnScreen)
        }
    }

    private fun checkOrderSummaryPreText(expectedText: String) {
        Assert.assertEquals("Order summary pre-text incorrect",
            expectedText,
            orderSummaryPreText.text)
    }

    private fun checkWhatHappensNextHeading() {
        Assert.assertEquals("What happens next heading incorrect",
            "What happens next",
            whatHappensNextHeading.text)
    }

    private fun checkPrescriptionHasBeenSentText() {
        Assert.assertEquals("Prescription has been sent text incorrect",
            "Your prescription request has been sent to your GP surgery.",
            prescriptionHasBeenSentText.text)
    }

    private fun checkSendToNominatedPharmacyText(pharmacyName: String, actual: String) {
        Assert.assertEquals("Advice for nominated pharmacy is incorrect",
            "Once a GP approves it, they'll send your prescription to your nominated pharmacy, $pharmacyName.",
            actual)
    }

    private fun checkOnlinePharmacyOncePreparedText(pharmacyName: String, actual: String) {
        Assert.assertEquals("Once prepared by online pharmacy text is incorrect",
            "When the pharmacists have checked and prepared your prescription, $pharmacyName will send it to" +
                " you in the post.",
            actual)
    }
}

