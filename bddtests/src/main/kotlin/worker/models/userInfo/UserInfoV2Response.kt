package worker.models.userInfo

data class UserInfoV2Response(val nhsLoginId: String,
                              val nhsNumber: String,
                              val odsCode: String,
                              val lastLogin: String)
