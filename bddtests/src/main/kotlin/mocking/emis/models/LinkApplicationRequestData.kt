package mocking.emis.models

import models.Patient
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "LinkApplicationRequest")
data class LinkApplicationRequestData(
        @field:XmlElement(name = "Surname") var surname: String,
        @field:XmlElement(name = "DateOfBirth") var dateOfBirth: String) {

    @field:XmlElement(name = "LinkageDetails")
    var linkageDetails: LinkageDetails? = null

    constructor(patient: Patient) : this(patient.name.surname, patient.age.dateOfBirth) {
        linkageDetails = LinkageDetails(patient.accountId, patient.linkageKey, patient.odsCode)
    }
}