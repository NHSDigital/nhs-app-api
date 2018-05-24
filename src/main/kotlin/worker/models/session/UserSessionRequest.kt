package worker.models.session

data class UserSessionRequest(var codeVerifier: String, var authCode: String?)
