package mocking.tpp.models

import javax.xml.bind.annotation.*

@XmlRootElement(name = "ListRepeatMedicationReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class ListRepeatMedicationReply (
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var onlineUserId: String = "default onlineUserId",
        @XmlAttribute var uuid: String = "default uuid",
        @XmlElement var Medication: MutableCollection<Medication> = mutableListOf()
)