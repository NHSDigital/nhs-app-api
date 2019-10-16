package models

data class ExpectedDocument (
    val id: String,
    val dateTypeAndSize: String,
    val date: String,
    val actions: List<String>,
    val documentTerm: String,
    var name: String? = null
)