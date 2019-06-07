package mocking.microtest.models

class ExceptionResponse(val internalResponseCode: Long,
                        exceptionMessage: String) {
    val internalResponseMessage = "Exception occurred during API processing."
    val exceptions = arrayOf(Exception(exceptionMessage))


    class Exception(val message: String) {
        val callStack = "Not available"
        val exceptionId = "00000000-0000-0000-0000-000000000000"
        val innerExceptionId = "00000000-0000-0000-0000-000000000000"
        val isRootException = true
    }
}