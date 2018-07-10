package worker.models.myrecord

data class ConsultationItem (
        val effectiveDate: Date,
        val consultationLocation: String,
        val consultationHeaders: MutableList<ConsultationHeaderItem>
)