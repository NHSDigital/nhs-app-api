package worker.models.appointments


data class SlotResponseObject(
        var id: String,
        var type: String,
        var startTime: String,
        var endTime: String,
        var location: String,
        var clinicians: Array<String?>,
        var channel: Int? = 0
)