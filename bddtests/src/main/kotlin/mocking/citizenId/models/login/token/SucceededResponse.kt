package mocking.citizenId.models.login.token

class SucceededResponse(accessToken: String, expiresIn: String, refreshExpiresIn: String, refreshToken: String, refreshType: String) {
    var access_token: String? = accessToken
    var expires_in: String? = expiresIn
    var refresh_expiresIn: String? = refreshExpiresIn
    var refresh_token: String? = refreshToken
    var refresh_type: String? = refreshType

}