package worker.models.myrecord

data class TppDcrEvents(
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<TppDcrEvent>
)
