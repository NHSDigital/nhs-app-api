package mocking.tpp.models

import mocking.defaults.TppMockDefaults
import models.Patient
import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "LinkAccount")
@XmlAccessorType(XmlAccessType.FIELD)
data class LinkAccount(
        @XmlAttribute var apiVersion: String = TppMockDefaults.TPP_API_VERSION,
        @XmlAttribute var accountId: String = "default accountId",
        @XmlAttribute var passphrase: String = "default passphrase",
        @XmlAttribute var lastName: String = "default lastName",
        @XmlAttribute var dateOfBirth: String = "1985-05-29T00:00:00.0Z",
        @XmlAttribute var organisationCode: String = "default unitId",
        @XmlAttribute var retrieveOnly: String = "n",
        @XmlAttribute var nhsNumber: String = "default nhsNumber",
        @XmlAttribute var emailAddress: String = "default email",
        @XmlAttribute var uuid: String = TppMockDefaults.DEFAULT_TPP_UUID,
        @field:XmlElement(name = "Application") var application: Application = TppMockDefaults.DEFAULT_TPP_APPLICATION
) {
        companion object {
            fun forPatient(patient: Patient): LinkAccount {
                    return LinkAccount(
                            accountId = patient.accountId,
                            passphrase = patient.linkageKey,
                            lastName = patient.name.surname,
                            dateOfBirth = patient.age.dateOfBirth,
                            organisationCode = patient.odsCode,
                            nhsNumber = patient.nhsNumbers.first().filter { !it.isWhitespace() },
                            emailAddress = patient.contactDetails.emailAddress
                    )
            }
        }
}
