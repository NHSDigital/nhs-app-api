package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "Session")
@XmlAccessorType(XmlAccessType.FIELD)
data class Session (
    @XmlAttribute var sessionId: String = "default sessionId",
    @XmlAttribute var type: String = "default type",
    @XmlAttribute var staffDetails: String = "default staffName",
    @XmlAttribute var location: String = "default location",
    @XmlElement var Slot: MutableCollection<Slot> = mutableListOf()
)
