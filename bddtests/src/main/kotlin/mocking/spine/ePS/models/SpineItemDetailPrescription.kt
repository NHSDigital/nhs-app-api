package mocking.spine.ePS.models


data class SpineItemDetailPrescription(
        var pendingCancellations: String,
        var prescriptionLastIssueDispensedDate: String,
        var prescriptionDownloadDate: String,
        var repeatInstance: RepeatInstance,
        var prescriptionDispensedDate: String,
        var prescriptionTreatmentType: String,
        var lastEventDate: String,
        var prescriptionClaimedDate: String,
        var lineItems: Map<String, LineItemDetail>,
        var nominatedPharmacy: Organisation,
        var patientNhsNumber: String,
        var prescriber: Organisation,
        var epsVersion: String,
        var prescriptionIssueDate: String,
        var prescriptionStatus: String,
        var dispensingPharmacy: Organisation
)