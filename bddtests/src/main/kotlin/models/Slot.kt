package models

data class Slot(
        var date: String = "",
        var time: String = "",
        var sessionName: String? = null,
        var slotType: String = "",
        var location: String = "",
        var clinicians: Set<String> = HashSet(),
        var id: Int? = null,
        var channel: String = "Unknown",
        var telephoneNumber: String = ""
)
