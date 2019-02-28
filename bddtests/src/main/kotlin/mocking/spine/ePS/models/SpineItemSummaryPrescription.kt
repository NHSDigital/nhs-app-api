package mocking.spine.ePS.models

data class SpineItemSummaryPrescription (
        var lastEventDate: String,
        var prescriptionIssueDate: String,
        var patientNhsNumber: String,
        var epsVersion: String,
        var currentIssueNumber: String? = null,
        var repeatInstance: RepeatInstance? = null,
        var pendingCancellations: String,
        var prescriptionTreatmentType: String,
        var prescriptionStatus: String,
        var lineItems: Map<String, String>
)
