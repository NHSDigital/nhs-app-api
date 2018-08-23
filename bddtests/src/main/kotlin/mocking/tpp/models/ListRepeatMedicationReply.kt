package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement
import javax.xml.bind.annotation.XmlElement

@XmlRootElement(name = "ListRepeatMedicationReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class ListRepeatMedicationReply (
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var onlineUserId: String = "default onlineUserId",
        @XmlAttribute var uuid: String = "default uuid",
        @XmlElement var Medication: MutableCollection<Medication> = mutableListOf()
)
