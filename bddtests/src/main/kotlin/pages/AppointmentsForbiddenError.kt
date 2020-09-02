package pages

open class AppointmentsForbiddenError: ForbiddenError() {

    private val coronaVirusHeader = setupElement("Get advice about coronavirus")
    private val gpAdviceHeader = setupElement("Ask your GP for advice")
    private val gpAdminHeader = setupElement("Additional GP services")
    private val nhs111Header = setupElement("Use NHS 111 online")

    fun assertGpAdviceMenuItem() : AppointmentsForbiddenError{
        gpAdviceHeader.assertIsVisible()
        return this
    }

    fun assertGpAdminMenuItem(): AppointmentsForbiddenError{
        gpAdminHeader.assertIsVisible()
        return this
    }

    fun assertCoronaVirusMenuItem(): AppointmentsForbiddenError{
        coronaVirusHeader.assertIsVisible()
        return this
    }

    fun assertNHS111Online(): AppointmentsForbiddenError{
        nhs111Header.assertIsVisible()
        return this
    }
}
