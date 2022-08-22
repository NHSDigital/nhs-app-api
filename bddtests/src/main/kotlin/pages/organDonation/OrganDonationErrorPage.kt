package pages.organDonation

import org.junit.Assert
import pages.ErrorPage
import pages.text

class OrganDonationErrorPage: ErrorPage() {

    val errorHeader = "Cannot access organ donation details"
    val errorMessageWithRetry = "If the problem persists you can contact NHS Blood and Transplant " +
            "to get help with this."
    val errorMessageNoRetry = "You cannot register your decision or update your organ donation preferences right now."
    val processingHeader = "Decision being processed"
    val processingMessage = "Check back later to confirm that your decision has been registered."
    val contactDetailsText = "If you keep seeing this message, email NHSApp.Enquiries@nhsbt.nhs.uk " +
            "(NHS Blood and Transplant) for help.";

    private val contactDetailsFinderFormat = "//div[@data-purpose='error']//div[@data-purpose='msg-contactdetails']"
    private val contactDetailsLocation = findElementByLocator(contactDetailsFinderFormat)

    fun assertContactDetails() {
        Assert.assertEquals("Contact Details message incorrect. ",
            contactDetailsText, contactDetailsLocation.text);
    }
}
