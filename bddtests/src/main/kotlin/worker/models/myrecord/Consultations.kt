package worker.models.myrecord

data class Consultations(
        val hasAccess: Boolean,
        val hasErrored: Boolean,
        val data: MutableList<ConsultationItem>
)
