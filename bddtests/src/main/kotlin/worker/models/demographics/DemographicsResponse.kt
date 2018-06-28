package worker.models.demographics

data class DemographicsResponse(
        var patientName: String? = null,
        var dateOfBirth: String? = null,
        var sex: String? = null,
        var address: String? = null,
        var nhsNumber: String? = null
)