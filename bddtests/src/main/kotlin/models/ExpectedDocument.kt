package models

data class ExpectedDocument (
        val id: String,
        val typeAndSize: String? = null,
        var date: String,
        val actions: List<String>,
        var term: String? = null,
        var name: String? = null
)
