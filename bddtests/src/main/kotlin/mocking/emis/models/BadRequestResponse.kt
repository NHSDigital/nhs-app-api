package mocking.emis.models

class BadRequestResponse(val message: String) {

    var modelState: Map<String, ArrayList<String>>? = null

    constructor(message: String, fieldName: String) : this(message) {
        modelState = mapOf(String.format("request.%1\$s", fieldName) to arrayListOf("An error has occurred."))
    }
}
