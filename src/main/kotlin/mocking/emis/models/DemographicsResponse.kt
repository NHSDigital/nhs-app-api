package mocking.emis.models

import java.time.LocalDateTime

data class DemographicsResponse(
        var title: String? = null,
        var firstName: String? = null,
        var surname: String? = null,
        var callingName: String? = null,
        var patientIdentifiers: MutableList<PatientIdentifier>? = null,
        var dateOfBirth: LocalDateTime? = LocalDateTime.MIN,
        var sex: Sex? = null,
        var contactDetails: ContactDetails? = null,
        var address: Address? = null
)