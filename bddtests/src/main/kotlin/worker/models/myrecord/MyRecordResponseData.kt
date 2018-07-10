package worker.models.myrecord

data class MyRecordResponseData(
        val hasSummaryRecordAccess: Boolean,
        val hasDetailedRecordAccess: Boolean,
        val supplier: String,
        val allergies: Allergies,
        var medications: Medications,
        var immunisations: Immunisations,
        var testResults: TestResults,
        var problems: Problems,
        var tppDcrEvents: TppDcrEvents,
        var consultations: Consultations
)

data class Medications(
    val hasAccess: Boolean,
    val hasErrored: Boolean,
    val data: MedicationsData
)

data class MedicationsData(
        val acuteMedications: MutableList<MedicationItem>,
        val currentRepeatMedications: MutableList<MedicationItem>,
        val discontinuedRepeatMedications: MutableList<MedicationItem>
)

data class MedicationItem(
       val date: String,
       val lineItems: MutableList<MedicationLineItem>
)

data class MedicationLineItem(
       val text: String,
       val lineItems: MutableList<String>
)


