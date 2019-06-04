package mocking.emis.models

class ErrorResponse(responseCode: String, message:String?) {
        val errorReference = "a4ab0d1c-7037-4a49-98f2-948d969d00b9"
        val message = message?: "Exception occurred during API processing."
        var internalResponseCode:String = responseCode

        constructor(responseCode:Int, message:String? = null):this
        (responseCode.toString(), message?: "Exception occurred during API processing.")
}