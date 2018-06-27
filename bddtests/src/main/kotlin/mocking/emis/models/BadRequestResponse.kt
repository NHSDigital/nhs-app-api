package mocking.emis.models

class BadRequestResponse(val message: String) {

    var modelState: Map<String, Iterable<String>>? = null

    constructor(message: String, fieldName: String) : this(message) {
        modelState = mapOf(String.format("request.%1\$s", fieldName) to listOf("An error has occurred."))
    }
}