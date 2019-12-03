package models.prescriptions

data class PrescriptionLoaderConfiguration(
        val noPrescriptions: Int,
        val noCourses: Int,
        val noRepeats: Int,
        val showDosage: Boolean = true,
        val showQuantity: Boolean = true)