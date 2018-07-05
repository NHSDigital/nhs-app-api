package mocking.emis.models

data class Session(
        var sessionName: String,
        var sessionId: Int,
        var locationId: Int? = null,
        var defaultDuration: Int? = null,
        var sessionType: SessionType? = null,
        var numberOfSlots: Int? = null,
        var clinicianIds: List<Int> = emptyList(),
        var startDate: String? = null,
        var endDate: String? = null
)
