package pages

open class AppointmentsGpSessionError: GpSessionError() {

    private val coronaVirusHeader = setupElement("Get advice about coronavirus");
    private val gpAdviceHeader = setupElement("Ask your GP for advice")
    private val gpAdminHeader = setupElement("Additional GP services")
    private val nhs111Header = setupElement("Use NHS 111 online")

    fun assertGpAdviceMenuItem() : AppointmentsGpSessionError{
        gpAdviceHeader.assertIsVisible()
        return this
    }

    fun assertGpAdminMenuItem(): AppointmentsGpSessionError{
        gpAdminHeader.assertIsVisible()
        return this
    }

    fun assertCoronaVirusMenuItem(): AppointmentsGpSessionError{
        coronaVirusHeader.assertIsVisible()
        return this
    }

    fun assertNHS111Online(): AppointmentsGpSessionError{
        nhs111Header.assertIsVisible()
        return this
    }
}
