package mocking.tpp.models

import java.util.*
import javax.xml.bind.annotation.*

@XmlRootElement(name = "RequestMedicationReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class RequestMedicationReply (
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var onlineUserId: String = "default onlineUserId",
        @XmlAttribute var uuid: String = "00000000-0000-0000-0000-000000000000",
        @XmlAttribute var message: String = "default message"
)