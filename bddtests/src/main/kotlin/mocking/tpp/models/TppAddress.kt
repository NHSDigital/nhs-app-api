package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute

@XmlAccessorType(XmlAccessType.FIELD)
data class TppAddress(
        @XmlAttribute var address: String = "default address",
        @XmlAttribute var addressType: String = "default address type"
)