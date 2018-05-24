package mocking.emis.models

import models.Patient

data class LinkApplicationRequestData(var Surname: String, var DateOfBirth: String) {
    var LinkageDetails: LinkageDetails? = null

    constructor(patient: Patient) : this(patient.surname, patient.dateOfBirth) {
        LinkageDetails = LinkageDetails(patient.accountId, patient.linkageKey, patient.odsCode)
    }
}