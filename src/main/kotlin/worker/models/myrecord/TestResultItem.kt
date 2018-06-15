package worker.models.myrecord

data class TestResultItem (
        val effectiveDate: Date,
        val term: String,
        val testResultLineItems: MutableList<String>
)