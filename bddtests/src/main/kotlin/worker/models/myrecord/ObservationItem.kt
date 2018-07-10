package worker.models.myrecord

data class ObservationItem (
        val term: String,
        val associatedTexts: MutableList<String>
)