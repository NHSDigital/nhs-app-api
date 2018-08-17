package mocking.emis.models

data class AddVerificationRequest (
        var nhsNumber: String,
        var nationalPracticeCode: String,
        var token: String,
        var additionalComment: String? = null)