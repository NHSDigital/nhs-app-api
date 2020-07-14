package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute

@XmlAccessorType(XmlAccessType.FIELD)
data class Application(
        @XmlAttribute var name: String = "default name",
        @XmlAttribute var version: String = "default version",
        @XmlAttribute var providerId: String = "default providerId",
        @XmlAttribute var deviceType: String = "default deviceType"
)
