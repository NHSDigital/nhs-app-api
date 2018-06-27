package worker.models.prescriptionsSubmission

data class PrescriptionSubmissionRequest(val courseIds: MutableList<String>, val specialRequest: String? = null)