package worker.models.userInfo

data class UserInfoResponse(val odsCode: String,
                            val nhsNumber: String,
                            val betaTester: Boolean)
