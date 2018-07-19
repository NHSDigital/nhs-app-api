package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlElement

@XmlRootElement(name = "ListSlotsReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class ListSlotsReply (
    @XmlAttribute var patientId: String = "default patientId",
    @XmlAttribute var onlineUserId: String = "default onlineUserId",
    @XmlAttribute var uuid: String = "default uuid",
    @XmlAttribute var bookableDays: String = "defualt bookableDays",
    @XmlElement var Session: MutableCollection<Session> = mutableListOf()
)