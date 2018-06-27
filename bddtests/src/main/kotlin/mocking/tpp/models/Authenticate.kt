package mocking.tpp.models

import javax.xml.bind.annotation.*

@XmlRootElement(name = "Authenticate")
@XmlAccessorType(XmlAccessType.FIELD)
data class Authenticate(
        @XmlAttribute var apiVersion: String = "default apiVersion",
        @XmlAttribute var accountId: String = "default accountId",
        @XmlAttribute var passphrase: String = "default passphrase",
        @XmlAttribute var unitId: String = "default unitId",
        @XmlAttribute var uuid: String = "default uuid",
        @field:XmlElement(name = "Application") var application: Application = Application(
                "default name",
                "default version",
                "default providerId",
                "default deviceType"
        )
)