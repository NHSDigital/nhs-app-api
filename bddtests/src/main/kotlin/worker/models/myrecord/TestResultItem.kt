package worker.models.myrecord

data class TestResultItem (
        val date: Date,
        val description: String,
        val testResultLineItems: MutableList<String>
)