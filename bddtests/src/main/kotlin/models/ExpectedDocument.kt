package models

data class ExpectedDocument (
    val id: String,
    val typeAndSize: String,
    val date: String,
    val actions: List<String>,
    var term: String? = null,
    var name: String? = null
)