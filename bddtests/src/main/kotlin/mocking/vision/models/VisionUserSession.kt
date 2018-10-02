package mocking.vision.models

data class VisionUserSession(
        var rosuAccountId: String,
        var apiKey: String,
        var odsCode: String,
        var patientId: String) {
    var provider: String
    var accountId: String

    init {
        val defaultProviderAccountId = "nhson001"
        provider = defaultProviderAccountId
        accountId = defaultProviderAccountId
    }
}