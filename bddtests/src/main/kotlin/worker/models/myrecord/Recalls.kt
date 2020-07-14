package worker.models.myrecord

data class Recalls(
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<RecallItem>
)
