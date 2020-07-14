package worker.models.myrecord

data class EncounterItem (
        val recordedOn: Date,
        val description: String,
        val value: String,
        val unit: String
)
