package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "ViewAppointmentsReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class ViewAppointmentsReply {
    @XmlAttribute var patientId: String = "default patientId",
    @XmlAttribute var onlineUserId: String = "default onlineUserId",
    @XmlAttribute var uuid: String = "default uuid",
    @XmlElement var Appointment: MutableCollection<Appointment> = mutableListOf()
}