package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "ListServiceAccessesReply")
@XmlAccessorType(XmlAccessType.FIELD)
data class ListServiceAccessesReply(
        @XmlAttribute var patientId: String = "default patientId",
        @XmlAttribute var onlineUserId: String = "default onlineUserId",
        @XmlAttribute var unitId: String = "default unitID",
        @field:XmlElement(name="ServiceAccess")
        @XmlElement var Services: MutableCollection<ServiceAccess> = mutableListOf()
                                   )
