package worker.models.myrecord

data class ConsultationHeaderItem (
        val header: String,
        val observations: MutableList<ObservationItem>
)
