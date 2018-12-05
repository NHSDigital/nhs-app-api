package worker.models.patient

data class Im1ConnectionToken(

        var im1CacheKey: String? = null,
        var accessIdentityGuid: String? = null,
        var accountId: String? = null,
        var passphrase: String? = null,
        var providerId: String? = null,
        var rosuAccountId: String? = null,
        var apiKey: String? = null

)
