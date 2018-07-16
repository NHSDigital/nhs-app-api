package worker.models.myrecord

data class MedicationLineItem(
        val text: String,
        val lineItems: MutableList<String>
)