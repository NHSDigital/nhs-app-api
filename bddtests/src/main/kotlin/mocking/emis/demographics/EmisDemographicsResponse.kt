package mocking.emis.demographics

import models.Patient

data class EmisDemographicsResponse(
        var title: String? = null,
        var firstName: String? = null,
        var surname: String? = null,
        var callingName: String? = null,
        var patientIdentifiers: MutableList<PatientIdentifier>? = null,
        var dateOfBirth: String? = null,
        var sex: Sex? = null,
        var contactDetails: ContactDetails? = null,
        var address: Address? = null
) {
    constructor(patient: Patient, patientIdentifiers: Array<PatientIdentifier>) : this(
            title = patient.title,
            firstName = patient.firstName,
            surname = patient.surname,
            callingName = patient.callingName,
            patientIdentifiers = patientIdentifiers.toMutableList(),
            dateOfBirth = patient.dateOfBirth,
            sex = patient.sex,
            contactDetails = patient.contactDetails,
            address = patient.address
    )
}