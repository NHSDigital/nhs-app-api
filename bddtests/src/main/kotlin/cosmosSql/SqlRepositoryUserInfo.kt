package cosmosSql

import com.fasterxml.jackson.annotation.JsonProperty

data class SqlRepositoryUserInfo(@get:JsonProperty("OdsCode") @JsonProperty("OdsCode") val OdsCode: String,
                                 @get:JsonProperty("NhsNumber") @JsonProperty("NhsNumber")val NhsNumber: String?)
