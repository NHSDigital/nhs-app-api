package worker.models.userDevices

data class UserDevice(
        val _id: String,
        val NhsLoginId: String,
        val RegistrationId: String,
        val PnsToken: String
)
