package worker.models.linkage

data class CreateLinkageRequest (
        var odsCode: String,
        var nhsNumber: String,
        var identityToken: String,
        var emailAddress: String)

