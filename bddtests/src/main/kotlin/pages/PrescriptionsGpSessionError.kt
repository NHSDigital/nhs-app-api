package pages

open class PrescriptionsGpSessionError: GpSessionError() {

    private val emergencyPrescriptionsHeader = setupSubHeader("Find out how to get an emergency prescription");

    fun assertEmergencyMenuItem() : PrescriptionsGpSessionError{
        emergencyPrescriptionsHeader.assertIsVisible()
        return this
    }
}
