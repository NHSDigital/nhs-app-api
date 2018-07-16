package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "Slot")
@XmlAccessorType(XmlAccessType.FIELD)
data class Slot {
    @XmlAttribute var startDate: String = "default startDate",
    @XmlAttribute var endDate: String = "default endDate",
    @XmlAttribute var type: String = "default type"
}