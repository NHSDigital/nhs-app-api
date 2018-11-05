package mocking.tpp.models

import mocking.defaults.TppMockDefaults
import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "Authenticate")
@XmlAccessorType(XmlAccessType.FIELD)
data class Authenticate(
        @XmlAttribute var apiVersion: String = TppMockDefaults.TPP_API_VERSION,
        @XmlAttribute var accountId: String = "default accountId",
        @XmlAttribute var passphrase: String = "default passphrase",
        @XmlAttribute var unitId: String = "default unitId",
        @XmlAttribute var uuid: String = "default uuid",
        @field:XmlElement(name = "Application") var application: Application = TppMockDefaults.DEFAULT_TPP_APPLICATION
)
