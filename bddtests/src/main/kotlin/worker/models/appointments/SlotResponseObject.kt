package worker.models.appointments

data class SlotResponseObject(
        var id: String,
        var type: String? = null,
        var sessionName: String? = null,
        var startTime: String? = null,
        var endTime: String? = null,
        var location: String? = null,
        var clinicians: Array<String?>,
        var channel: Int = 0
)
