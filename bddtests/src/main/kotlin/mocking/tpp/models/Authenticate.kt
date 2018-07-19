package mocking.tpp.models

import mocking.defaults.MockDefaults
import javax.xml.bind.annotation.*

@XmlRootElement(name = "Authenticate")
@XmlAccessorType(XmlAccessType.FIELD)
data class Authenticate(
        @XmlAttribute var apiVersion: String = MockDefaults.TPP_API_VERSION,
        @XmlAttribute var accountId: String = "default accountId",
        @XmlAttribute var passphrase: String = "default passphrase",
        @XmlAttribute var unitId: String = "default unitId",
        @XmlAttribute var uuid: String = "default uuid",
        @field:XmlElement(name = "Application") var application: Application = MockDefaults.DEFAULT_TPP_APPLICATION
)