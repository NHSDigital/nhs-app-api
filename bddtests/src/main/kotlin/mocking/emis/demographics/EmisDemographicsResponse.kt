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
            title = patient.name.title,
            firstName = patient.name.firstName,
            surname = patient.name.surname,
            callingName = patient.name.callingName,
            patientIdentifiers = patientIdentifiers.toMutableList(),
            dateOfBirth = patient.age.dateOfBirth,
            sex = patient.sex,
            contactDetails = ContactDetails(
                    telephoneNumber = patient.contactDetails.telephoneFirst,
                    mobileNumber = patient.contactDetails.telephoneSecond,
                    emailAddress = patient.contactDetails.emailAddress),
            address = patient.contactDetails.address
    )
}
