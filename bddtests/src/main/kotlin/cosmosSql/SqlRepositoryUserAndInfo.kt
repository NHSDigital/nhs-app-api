package cosmosSql

import com.fasterxml.jackson.annotation.JsonProperty

data class SqlRepositoryUserAndInfo(
    @get:JsonProperty("Info") @JsonProperty("Info") val Info: SqlRepositoryUserInfo,
    @get:JsonProperty("id") @JsonProperty("id") val id: String)
