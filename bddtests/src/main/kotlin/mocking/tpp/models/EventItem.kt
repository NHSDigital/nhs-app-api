package mocking.tpp.models

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "Item")
@XmlAccessorType(XmlAccessType.FIELD)
data class EventItem(
        @XmlAttribute var type: String = "default type",
        @XmlAttribute var details: String = "default details"
)
