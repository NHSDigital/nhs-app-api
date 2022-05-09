package pages

open class AppointmentsGpSessionError: GpSessionError() {

    private val gpAdviceHeader = setupSubHeader("Ask your GP for advice")
    private val gpMedicalAdviceHeader = setupSubHeader("Ask your GP for medical advice")
    private val gpAdminHeader = setupSubHeader("Additional GP services")
    private val nhs111Header = setupSubHeader("Use NHS 111 online")
    private val askGpSurgeryAQuestionHeader = setupSubHeader("Ask your GP surgery a question")

    fun assertGpAdviceMenuItem() : AppointmentsGpSessionError{
        gpAdviceHeader.assertIsVisible()
        return this
    }

    fun assertGpMedicalAdviceMenuItem() : AppointmentsGpSessionError{
        gpMedicalAdviceHeader.assertIsVisible()
        return this
    }

    fun assertGpAdminMenuItem(): AppointmentsGpSessionError{
        gpAdminHeader.assertIsVisible()
        return this
    }

    fun assertNHS111Online(): AppointmentsGpSessionError{
        nhs111Header.assertIsVisible()
        return this
    }

    fun assertAskYourGpSurgeryAQuestionMenuItem(): AppointmentsGpSessionError{
        askGpSurgeryAQuestionHeader.assertIsVisible()
        return this
    }
}
