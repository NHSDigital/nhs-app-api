package models

data class Slot(
        var date: String = "",
        var time: String = "",
        var session: String = "",
        var location: String = "",
        var clinicians: Set<String> = HashSet(),
        var id: Int? = null
)
