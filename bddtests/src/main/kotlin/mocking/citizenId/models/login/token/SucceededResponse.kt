package mocking.citizenId.models.login.token

data class SucceededResponse(val access_token: String,
                             val token_type: String,
                             val expires_in: String,
                             val scope: String,
                             val id_token: String,
                             val refresh_token: String)
