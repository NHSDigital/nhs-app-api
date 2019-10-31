package worker.models.userDevices

data class InvalidUserDevice(
        val _id: String,
        val NhsLoginId: String,
        val invalidField: String
)
