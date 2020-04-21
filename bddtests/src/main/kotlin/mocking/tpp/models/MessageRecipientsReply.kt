package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlValue

@XmlRootElement(name = "MessageRecipientsReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class MessageRecipientsReply(
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var onlineUserId: String = "default onlineUserId",
        @XmlAttribute var uuid: String = "default uuid",
        @field:XmlElement(name="Item")
        @XmlElement var Item: MutableCollection<TppRecipient> = mutableListOf()
)

@XmlRootElement(name = "Item")
@XmlAccessorType(XmlAccessType.FIELD)
data class TppRecipient(
        @XmlAttribute var id: String = "default Id",
        @XmlAttribute var description: String = "default description",
        @XmlValue var value: String = "default name"
)
