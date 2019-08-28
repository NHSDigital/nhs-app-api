package mocking.citizenId.models.notifications

data class SuccessResponse(
        var jwks_uri: String = "http://stubs.local.bitraft.io:8080/citizenid/.well-known/jwks.json"
)