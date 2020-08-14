package mocking.emis.models

class ExceptionResponse(val internalResponseCode: String,
                        exceptionMessage: String) {
    val message = exceptionMessage
    val internalResponseMessage = exceptionMessage
    val exceptions = arrayOf(Exception(exceptionMessage))

    constructor(internalResponseCode: Long,
                exceptionMessage: String) : this(internalResponseCode.toString(), exceptionMessage)

    class Exception(val message: String) {
        val exceptionId = "00000000-0000-0000-0000-000000000000"
        val innerExceptionId = "00000000-0000-0000-0000-000000000000"
    }
}
