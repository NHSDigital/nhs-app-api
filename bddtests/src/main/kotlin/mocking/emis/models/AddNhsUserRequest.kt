package mocking.emis.models

data class AddNhsUserRequest (
        var nationalPracticeCode: String,
        var nhsNumber: String,
        var emailAddress: String?)
