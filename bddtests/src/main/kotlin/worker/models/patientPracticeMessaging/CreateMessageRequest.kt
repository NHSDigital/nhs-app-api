package worker.models.patientPracticeMessaging

data class CreateMessageRequest(val subject: String? = null, val messageBody: String, val recipient: String)