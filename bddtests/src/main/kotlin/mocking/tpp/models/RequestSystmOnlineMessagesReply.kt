package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "RequestSystmOnlineMessagesReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class RequestSystmOnlineMessagesReply (
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var onlineUserId: String = "default onlineUserId",
        @XmlAttribute var uuid: String = "default uuid",
        @XmlAttribute(name = "BookAppointments") var bookAppointments: String? = "default BookAppointments"
)