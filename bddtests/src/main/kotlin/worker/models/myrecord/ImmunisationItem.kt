package worker.models.myrecord

data class ImmunisationItem (
        val term: String,
        val effectiveDate: Date,
        val nextDate: String,
        val status: String
)
