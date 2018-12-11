package mocking.vision.models.error

data class VisionError (
        val code: String,
        val category: String = "",
        val text: String = "",
        val diagnostic: String = ""
)
