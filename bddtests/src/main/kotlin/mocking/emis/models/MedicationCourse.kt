package mocking.emis.models

data class MedicationCourse(
        val medicationCourseGuid : String,
        val name: String,
        val dosage: String?,
        val quantityRepresentation: String?,
        var prescriptionType: PrescriptionType,
        val constituents: List<String>,
        var canBeRequested: Boolean)

{
    fun getInstructionsText() : String {
        return pages.prescription.resolveDetailsField(dosage, quantityRepresentation)
    }
}
