package pages.appointments

import mocking.stubs.appointments.factories.AppointmentsBookingFactory
import models.Slot
import net.serenitybdd.core.Serenity
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

    fun checkAppointmentDetails() {
        val expectedSlot =
                Serenity.sessionVariableCalled<Slot>(AppointmentsBookingFactory.selectedSlot).copy(id = null)

        val areCliniciansExpected = expectedSlot.clinicians.isNotEmpty()
        val appointmentDetails = getAppointmentSlot(areCliniciansExpected)

        Assert.assertEquals(expectedSlot.clinicians, appointmentDetails.clinicians)
        Assert.assertEquals(expectedSlot.date, appointmentDetails.date)
        Assert.assertEquals(expectedSlot.location, appointmentDetails.location)
        Assert.assertEquals(expectedSlot.sessionName, appointmentDetails.sessionName)
        Assert.assertEquals(expectedSlot.slotType, appointmentDetails.slotType)
        Assert.assertEquals(expectedSlot.telephoneNumber, appointmentDetails.telephoneNumber)
        Assert.assertEquals(expectedSlot.time, appointmentDetails.time)
        Assert.assertEquals(expectedSlot.id, appointmentDetails.id)
    }
}
