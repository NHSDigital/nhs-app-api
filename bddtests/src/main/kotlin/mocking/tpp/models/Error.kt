package mocking.tpp.models

import mocking.defaults.TppMockDefaults
import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "Error")
@XmlAccessorType(XmlAccessType.FIELD)
data class Error(
        @XmlAttribute var errorCode: String = "default errorCode",
        @XmlAttribute var userFriendlyMessage: String = "default userFriendlyMessage",
        @XmlAttribute var uuid: String? = TppMockDefaults.DEFAULT_TPP_UUID
)
