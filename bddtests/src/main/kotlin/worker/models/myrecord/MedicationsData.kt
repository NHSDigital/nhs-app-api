package worker.models.myrecord

data class MedicationsData(
        val acuteMedications: MutableList<MedicationItem>,
        val currentRepeatMedications: MutableList<MedicationItem>,
        val discontinuedRepeatMedications: MutableList<MedicationItem>
)
