package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "ListSlots")
@XmlAccessorType(XmlAccessType.FIELD)
data class ListSlots (
    @XmlAttribute var apiVersion: String = "default apiVersion",
    @XmlAttribute var patientId: String = "default patientId",
    @XmlAttribute var onlineUserId: String = "default onlineUserId",
    @XmlAttribute var unitId: String = "default unitId",
    @XmlAttribute var uuid: String = "default uuid",
    @XmlAttribute var startDate: String = "default startDate",
    @XmlAttribute var numberOfDays: String = "numberOfDays"
)
