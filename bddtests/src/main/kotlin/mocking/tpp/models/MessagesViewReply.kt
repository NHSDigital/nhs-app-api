package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "MessagesViewReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class MessagesViewReply(
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var onlineUserId: String = "default onlineUserId",
        @XmlAttribute var uuid: String = "default uuid",
        @field:XmlElement(name="Message")
        @XmlElement var Message: MutableCollection<Message> = mutableListOf()
)

@XmlRootElement(name = "Message")
@XmlAccessorType(XmlAccessType.FIELD)
data class Message(
        @XmlAttribute var messageId: String = "default patientId",
        @XmlAttribute var conversationId: String = "default onlineUserId",
        @XmlAttribute var messageText: String = "default uuid",
        @XmlAttribute var read: String? = null,
        @XmlAttribute var deleted: String? = null,
        @XmlAttribute var incoming: String = "y",
        @XmlAttribute var binaryDataId: String? = null,
        @XmlAttribute var recipient: String = "default recipient",
        @XmlAttribute var sender: String = "default sender",
        @XmlAttribute var staffOrGroupOrTeamName: String = "default staffOrGroupOrTeamName",
        @XmlAttribute var sent: String = "default sent"
)
