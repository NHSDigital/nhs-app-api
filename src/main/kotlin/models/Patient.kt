package models

data class Patient(
        val title: String = "",
        val firstName: String = "",
        val surname: String = "",
        val dateOfBirth: String = "",
        val accountId: String = "",
        val odsCode: String = "",
        val connectionToken: String = "",
        val sessionId: String = "",
        val endUserSessionId: String = "",
        val linkageKey: String = "",
        val userPatientLinkToken: String = "",
        val nhsNumbers: List<String> = emptyList()
)
