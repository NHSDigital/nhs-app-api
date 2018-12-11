package mocking.vision.models.linkage

data class LinkageKeyGetResponse (
        val accountId: String,
        val linkageKey: String,
        val odsCode: String
)