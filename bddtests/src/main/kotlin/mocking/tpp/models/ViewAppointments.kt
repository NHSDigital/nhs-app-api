package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "ViewAppointments")
@XmlAccessorType(XmlAccessType.FIELD)
data class ViewAppointments(
    @XmlAttribute var apiVersion: String = "default apiVersion",
    @XmlAttribute var patientId: String = "default patientId",
    @XmlAttribute var onlineUserId: String = "default onlineUserId",
    @XmlAttribute var unitId: String = "default unitId",
    @XmlAttribute var uuid: String = "default uuid",
    @XmlAttribute var futureAppointments: String = "default futureAppointments"
)