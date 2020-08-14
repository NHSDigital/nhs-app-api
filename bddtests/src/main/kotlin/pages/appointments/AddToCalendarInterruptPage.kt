package pages.appointments

import pages.HybridPageElement
import pages.assertIsVisible

open class AddToCalendarInterruptPage : AppointmentSharedElementsPage() {

    private val addToCalendarHeaderMessage = "If this appointment changes, you'll have to update your calendar yourself"

    private val headerMessage = HybridPageElement(
            webDesktopLocator = "//h1[contains(text(),\"$addToCalendarHeaderMessage\")]",
            page = this
    )

    private val addToCalendarButton = HybridPageElement(
            webDesktopLocator = "//*[@id='addToCalendarButton']",
            page = this
    )

    override val titleText: String = "N/A"

    fun checkAddToCalendarMessage() {
        headerMessage.assertIsVisible()
    }

    fun clickAddToCalendarButton() {
        addToCalendarButton.click()
    }
}
