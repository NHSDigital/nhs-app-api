package worker.models.myrecord

data class TestResultItem (
        val date: Date,
        val description: String,
        val associatedTexts: MutableList<String>,
        val testResultChildLineItems: MutableList<TestResultChildLineItem>
)
