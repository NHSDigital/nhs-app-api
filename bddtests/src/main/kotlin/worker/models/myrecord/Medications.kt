package worker.models.myrecord

data class Medications(
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MedicationsData
)
