package mocking.tpp.models

import mocking.defaults.MockDefaults
import models.Patient
import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlElement

@XmlRootElement(name = "LinkAccount")
@XmlAccessorType(XmlAccessType.FIELD)
data class LinkAccount(
        @XmlAttribute var apiVersion: String = MockDefaults.TPP_API_VERSION,
        @XmlAttribute var accountId: String = "default accountId",
        @XmlAttribute var passphrase: String = "default passphrase",
        @XmlAttribute var lastName: String = "default lastName",
        @XmlAttribute var dateOfBirth: String = "1985-05-29T00:00:00.0Z",
        @XmlAttribute var organisationCode: String = "default unitId",
        @XmlAttribute var uuid: String = "default uuid",
        @field:XmlElement(name = "Application") var application: Application = MockDefaults.DEFAULT_TPP_APPLICATION
) {
        companion object {
            fun forPatient(patient: Patient): LinkAccount {
                    return LinkAccount(
                            accountId = patient.accountId,
                            passphrase = patient.linkageKey,
                            lastName = patient.surname,
                            dateOfBirth = patient.dateOfBirth,
                            organisationCode = patient.odsCode,
                            uuid = MockDefaults.DEFAULT_TPP_UUID
                    )
            }
        }
}
