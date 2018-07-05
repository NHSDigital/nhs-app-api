package models.prescriptions

import mocking.emis.models.PrescriptionType

data class MedicationCourse(
        val medicationCourseGuid : String,
        val name: String,
        val dosage: String?,
        val quantityRepresentation: String? = null,
        var prescriptionType: PrescriptionType? = null,
        val constituents: List<String>? = null,
        var canBeRequested: Boolean? = null)

{
    fun getInstructionsText() : String {
        return pages.prescription.resolveDetailsField(dosage, quantityRepresentation)
    }
}
