package pages.appointments

import org.junit.Assert
import pages.HybridPageElement
import pages.text

open class CancellingSuccessPage : AppointmentSharedElementsPage() {

    private val cancellationSuccessMessage = "Your GP appointment has been cancelled"

    private val successMessage = HybridPageElement(
            webDesktopLocator ="//h1[contains(text(),\"$cancellationSuccessMessage\")]",
            page = this
    )

    override val titleText: String = "Your GP appointment has been cancelled"

    fun checkCancelSuccessMessage() {
        Assert.assertEquals(cancellationSuccessMessage, successMessage.text)
    }
}
