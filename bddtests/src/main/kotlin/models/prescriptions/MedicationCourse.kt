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
    fun getInstructions() : ArrayList<String> {
        val instrictionsList = pages.prescription.resolveDetailsField(dosage, quantityRepresentation)
        return when (instrictionsList.size != 0){
            true -> instrictionsList
            false -> arrayListOf("")
        }
    }

    fun getInstructionsText() : String {
        return getInstructions().joinToString(" - ")
    }
}
