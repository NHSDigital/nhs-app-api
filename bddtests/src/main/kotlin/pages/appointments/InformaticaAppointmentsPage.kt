package pages.appointments

import pages.HybridPageObject
import pages.navigation.HeaderNative
import pages.sharedElements.expectedPage.ExpectedPageStructure

class InformaticaAppointmentsPage : HybridPageObject() {

    private lateinit var headerNative: HeaderNative

    fun isLoaded() {
        headerNative.waitForPageHeaderText("Service unavailable")
    }

    fun assertInformaticaAppointmentsPageVisible() {
        val expected = ExpectedPageStructure()
                .h2("Appointment booking is not currently available directly through the NHS App")
                .paragraph("Your GP surgery uses Appointments Online to book appointments, " +
                        "and you’ll need a username and password from your GP surgery.")
                .paragraph("If you have a username and password, log in to Appointments Online .")
        expected.assert(this)
    }
}