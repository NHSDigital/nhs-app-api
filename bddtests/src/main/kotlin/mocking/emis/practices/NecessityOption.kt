package mocking.emis.practices

enum class NecessityOption(val text: String) {
    NOT_ALLOWED("NotRequested") {
        override fun toString() = text
    },
    OPTIONAL("RequestedOptional") {
        override fun toString() = text
    },
    MANDATORY("RequestedMandatory") {
        override fun toString() = text
    }
}