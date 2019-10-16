package worker.models.userInfo

data class UserAndInfoResponse(val nhsLoginId: String,
                               val info: UserInfoResponse,
                               val timeStamp: String)

