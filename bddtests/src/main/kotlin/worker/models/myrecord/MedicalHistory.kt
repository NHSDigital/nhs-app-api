package worker.models.myrecord

data class MedicalHistory(
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<MedicalHistoryItem>
)
