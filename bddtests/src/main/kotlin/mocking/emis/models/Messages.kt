package mocking.emis.models

data class Messages(
        var welcomeMessage: String = "",
        var appointmentsMessage: String? = "Thank you for using our online service. " +
                                          "This is a new service and we welcome your feedback " +
                                          "- please contact us via our website.\n" +
                "\n" +
                "Please do not book appointments if you have a sort throat!",
        var prescribingMessage: String = ""
)
