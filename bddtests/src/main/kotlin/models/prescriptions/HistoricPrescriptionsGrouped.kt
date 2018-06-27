package models.prescriptions

data class HistoricPrescriptionsGrouped(
        val rejectedPrescriptions: List<HistoricPrescription>,
        val requestedPrescriptions: List<HistoricPrescription>,
        val approvedPrescriptions: List<HistoricPrescription>)