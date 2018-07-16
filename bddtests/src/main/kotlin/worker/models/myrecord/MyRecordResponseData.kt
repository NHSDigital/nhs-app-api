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
