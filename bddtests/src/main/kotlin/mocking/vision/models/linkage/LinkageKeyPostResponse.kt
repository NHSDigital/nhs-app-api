package mocking.vision.models.linkage

data class LinkageKeyPostResponse (
        val accountId: String,
        val linkageKey: String,
        val odsCode: String,
        val surname: String,
        val dateOfBirth: String,
        val apiKey: String
)
