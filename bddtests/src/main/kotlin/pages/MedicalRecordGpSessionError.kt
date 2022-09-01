package pages

open class MedicalRecordGpSessionError: GpSessionError() {

    private val medicalRecordHeader = setupHeader("Cannot show GP health record");

    fun assertMedicalRecordHeader() : MedicalRecordGpSessionError{
        medicalRecordHeader.assertIsVisible()
        return this
    }
}
