package worker.models.patientPracticeMessaging

data class CreateMessageRequest(val subject: String, val messageBody: String, val recipient: String)