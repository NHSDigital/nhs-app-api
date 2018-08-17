package mocking.emis.models

class ErrorResponse(val internalResponseCode: Int) {
        val message = "Exception occurred during API processing."
        val errorReference = "a4ab0d1c-7037-4a49-98f2-948d969d00b9"
}