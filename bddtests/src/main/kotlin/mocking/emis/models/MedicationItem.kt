package mocking.emis.models

data class MedicationItem (
        var firstIssueDate: String? = null,
        var prescriptionType: String,
        var drugStatus: String,
        var term: String,
        var isMixture: Boolean,
        var mixture: MedicationMixture? = null,
        var dosage: String,
        var quantityRepresentation: String,
        var lastIssueDate: String? = null
)
