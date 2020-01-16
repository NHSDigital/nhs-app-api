package pages.patientPracticeMessaging

import org.junit.Assert
import pages.HybridPageElement
import pages.HybridPageObject
import pages.appointments.findByXpath
import pages.assertIsVisible
import pages.navigation.WebHeader
import pages.text

class PatientPracticeMessagingContactYourGpPage: HybridPageObject() {

    private lateinit var header: WebHeader

    private val expectedHeaderText = "Phone or visit your GP or NHS 111"
    private val expectedMessagingPurposeText = "Messaging is for non-urgent advice."
    private val expectedWhatToDoNextText = "For advice now, phone or visit your GP surgery, " +
            "go to 111.nhs.uk or call 111."
    private val expectedCareCardHeaderText = "Urgent advice:\nCall 999 now if you have:"

    private val careCardLocator = "//div[@id='phoneYourGpCareCard']"

    private var messagingPurposeParagraph = HybridPageElement(
            "//p[@id='infoMessagingPurpose']",
            page = this,
            helpfulName = "Messaging is not for urgent advice info"
    )

    private var whatToDoNextParagraph = HybridPageElement(
            "//p[@id='infoWhatToDo']",
            page = this,
            helpfulName = "What to do next info"
    )

    private var careCard = HybridPageElement(
            careCardLocator,
            page = this,
            helpfulName = "Phone your GP for urgent advice Care Card"
    )

    private var careCardHeader = HybridPageElement(
            "$careCardLocator//p",
            page = this,
            helpfulName = "Phone your GP for urgent advice Care Card heading"
    )

    private var careCardSymptomListItem = HybridPageElement(
            "$careCardLocator//ul//li",
            page = this,
            helpfulName = "Phone your GP for urgent advice Care Card symptom"
    )

    fun assertIsDisplayed() {
        Assert.assertEquals(
                "Header should contain text $expectedHeaderText",
                expectedHeaderText,
                header.getPageTitle().text)
    }

    fun assertMessagingPurposeText() {
        messagingPurposeParagraph.waitForElement().assertIsVisible()
        Assert.assertEquals(
                "Text stating messaging is not for urgent advice not found",
                expectedMessagingPurposeText,
                messagingPurposeParagraph.textValue)
    }

    fun assertWhatToDoNextText() {
        whatToDoNextParagraph.waitForElement().assertIsVisible()
        Assert.assertEquals(
                "Text outlining what to do next not found",
                expectedWhatToDoNextText,
                whatToDoNextParagraph.textValue)
    }

    fun assertCareCardContent(expectedCareCardContent: List<Pair<String, String>>) {
        careCard.waitForElement().assertIsVisible()

        val actualSymptoms = arrayListOf<Pair<String, String>>()
        careCardSymptomListItem.elements.forEach {
            val strongText = it.findByXpath(".//strong")!!.text
            val fullText = it.text
            actualSymptoms.add(Pair(strongText, fullText))
        }

        Assert.assertEquals(
                "Expected care card symptoms not found",
                expectedCareCardContent,
                actualSymptoms)
        Assert.assertEquals(
                "Care card header not found",
                expectedCareCardHeaderText,
                careCardHeader.elements.first().text)
    }
}