package worker.models.myrecord

data class MedicationItem(
        val date: String,
        val lineItems: MutableList<MedicationLineItem>
)