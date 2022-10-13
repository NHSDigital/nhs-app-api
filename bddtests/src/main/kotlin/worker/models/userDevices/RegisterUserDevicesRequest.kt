package worker.models.userDevices

data class RegisterUserDevicesRequest(var devicePns :String, var deviceType: String, var installationId: String)
