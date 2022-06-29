package cosmosSql

import com.fasterxml.jackson.annotation.JsonProperty

data class SqlRepositoryCommsSender(val id: String,
                                    @get:JsonProperty("Name") @JsonProperty("Name")val Name: String,
                                    @get:JsonProperty("Timestamp") @JsonProperty("Timestamp")val TimeStamp: String)
