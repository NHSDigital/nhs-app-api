package mocking.tpp.models

import javax.xml.bind.annotation.*

@XmlRootElement(name = "Item")
@XmlAccessorType(XmlAccessType.FIELD)
data class EventItem(
        @XmlAttribute var type: String = "default type",
        @XmlAttribute var details: String = "default details"
)