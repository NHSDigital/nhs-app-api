package pages.appointments

import org.junit.Assert
import pages.HybridPageElement
import pages.text

open class BookingSuccessPage : AppointmentSharedElementsPage() {

    private val bookingSuccessMessage = "Your GP appointment has been booked"

    private val successMessage = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(),\"$bookingSuccessMessage\")]",
            page = this
    )

    override val titleText: String = "Your GP appointment has been booked"

    fun checkBookingSuccessMessage() {
        Assert.assertEquals(bookingSuccessMessage, successMessage.text)
    }
}
