package worker.models.myrecord

data class TestResults(
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<TestResultItem>
)
