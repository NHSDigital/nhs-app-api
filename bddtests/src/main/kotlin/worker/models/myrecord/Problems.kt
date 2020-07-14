package worker.models.myrecord

data class Problems(
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<ProblemItem>
)
