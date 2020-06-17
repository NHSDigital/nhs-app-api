package mocking.vision.models.error

enum class VisionErrorTypes (val Code: String, val Description: String){
    ACCESSDENIED("-35", "AccessDenied"),
    GATEWAYCONNECTIONTOEXTERNALSERVICEFAILED("-100", "Connection to external service failed"),
    GATEWAYUNKNOWNERROR("-100", "Unknown Error"),
    INVALIDPARAMETERPROVIDED("-31", "Invalid parameter provided"),
    INVALIDUSERCREDENTIALS("-30", "Invalid user credentials"),
    NOMATCH("-33", "No Match: couldn't link account with detail provided"),
    RECORDCURRENTLYUNAVAILABLE("-15", "Record currently unavailable - please try again later or contact " +
            "your Practice: VOSUsers record is locked, is patient selected in registration?"),
    REGISTRATIONINCOMPLETE("-8", "Registration incomplete"),
    USERHASALREADYBEENREGISTERED("-2", "User has already been registered")
}