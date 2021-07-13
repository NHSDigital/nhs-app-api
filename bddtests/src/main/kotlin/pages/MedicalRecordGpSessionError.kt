package pages

open class MedicalRecordGpSessionError: GpSessionError() {

    private val medicalRecordHeader = setupHeader("Sorry, your GP health record is unavailable");

    fun assertMedicalRecordHeader() : MedicalRecordGpSessionError{
        medicalRecordHeader.assertIsVisible()
        return this
    }
}
