package pages

import org.junit.Assert

open class AppointmentsGpSessionError: GpSessionError() {

    private val coronaVirusHeader = setupElement("Get advice about coronavirus");
    private val gpAdviceHeader = setupElement("Ask your GP for advice")
    private val gpAdminHeader = setupElement("Additional GP services")
    private val nhs111Header = setupElement("Use NHS 111 online")

    fun assertGpAdviceMenuItem() : AppointmentsGpSessionError{
        Assert.assertTrue(gpAdviceHeader.isVisible)
        return this
    }

    fun assertGpAdminMenuItem(): AppointmentsGpSessionError{
        Assert.assertTrue(gpAdminHeader.isVisible)
        return this
    }

    fun assertCoronaVirusMenuItem(): AppointmentsGpSessionError{
        Assert.assertTrue(coronaVirusHeader.isVisible)
        return this
    }

    fun assertNHS111Online(): AppointmentsGpSessionError{
        Assert.assertTrue(nhs111Header.isVisible)
        return this
    }
}
