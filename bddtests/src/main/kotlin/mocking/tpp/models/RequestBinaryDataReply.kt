package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "RequestBinaryDataReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class RequestBinaryDataReply(
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var onlineUserId: String = "default onlineUserId",
        @XmlAttribute var uuid: String = "default uuid",
        @field:XmlElement(name="BinaryData")
        @XmlElement var event: BinaryData = BinaryData()
)
