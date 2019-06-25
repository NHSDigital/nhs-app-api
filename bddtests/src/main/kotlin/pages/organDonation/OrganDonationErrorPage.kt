package pages.organDonation

import org.junit.Assert
import org.openqa.selenium.By
import pages.ErrorPage

class OrganDonationErrorPage: ErrorPage() {

    val errorHeader = "Something went wrong"
    val errorMessageWithRetry = "If the problem persists you can contact NHS Blood and Transplant " +
            "to get help with this."
    val errorMessageNoRetry = "You can contact NHS Blood and Transplant to get help with this."
    val processingHeader = "Decision being processed"
    val processingMessage = "Check back later to confirm that your decision has been registered."

    private val contactDetailsFinderFormat = "//div[@data-purpose='error']//div[@data-purpose='msg-contactdetails']"
    private val contactDetailsLocation = findElementByLocator(contactDetailsFinderFormat)
    private val contactDetailsText = arrayOf("Email", "NHSApp.Enquiries@nhsbt.nhs.uk")

    fun assertContactDetails() {
        contactDetailsLocation.actOnTheElement {
            val actualText = it.findElements(By.xpath(".//p"))
                    .map { element -> element.text }.toTypedArray()


            contactDetailsText.forEach { expected ->
                Assert.assertTrue("Expected to contain: '$expected'. Actual: '${actualText.joinToString()}'",
                        actualText.contains(expected))
            }
        }
    }
}
