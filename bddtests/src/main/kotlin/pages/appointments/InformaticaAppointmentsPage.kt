package pages.appointments

import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.TextBlockElement

class InformaticaAppointmentsPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Service unavailable")
    }

    fun assertInformaticaAppointmentsPageVisible() {
        TextBlockElement.withH2Header("Appointment booking is not currently " +
                "available directly through the NHS App", this)
                .assert("Your GP surgery uses Appointments Online to book appointments, " +
                        "and you’ll need a username and password from your GP surgery.")
    }
}