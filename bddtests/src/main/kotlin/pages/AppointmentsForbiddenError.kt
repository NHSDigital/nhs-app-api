package pages

open class AppointmentsForbiddenError: ForbiddenError() {

    private val nhs111Header = setupElement("Use NHS 111 online")

    fun assertNHS111Online(): AppointmentsForbiddenError{
        nhs111Header.assertIsVisible()
        return this
    }
}
