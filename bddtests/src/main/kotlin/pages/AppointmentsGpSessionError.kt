package pages

open class AppointmentsGpSessionError: GpSessionError() {

    private val gpMedicalAdviceHeader = setupSubHeader("Ask your GP for medical advice")
    private val nhs111Header = setupSubHeader("Use NHS 111 online")
    private val askGpSurgeryAQuestionHeader = setupSubHeader("Ask your GP surgery a question")

    fun assertGpMedicalAdviceMenuItem() : AppointmentsGpSessionError{
        gpMedicalAdviceHeader.assertIsVisible()
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
