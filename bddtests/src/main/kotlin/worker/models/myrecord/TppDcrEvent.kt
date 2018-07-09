package worker.models.myrecord

data class TppDcrEvent (
        val date: String,
        val locationAndDoneBy: String,
        val eventItems: MutableList<String>
)