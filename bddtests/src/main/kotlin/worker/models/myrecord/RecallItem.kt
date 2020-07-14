package worker.models.myrecord

data class RecallItem (
        val recordDate: Date,
        val name: String,
        val description: String,
        val result: String,
        val nextDate: String,
        val status: String
)
