package mocking.tpp.models

import javax.xml.bind.annotation.*

@XmlRootElement(name = "Event")
@XmlAccessorType(XmlAccessType.FIELD)
data class Event(
        @XmlAttribute var date: String = "default date",
        @XmlAttribute var doneBy: String = "default done by",
        @XmlAttribute var location: String = "default location",
        @field:XmlElement(name="Item")
        @XmlElement var Item: MutableCollection<EventItem> = mutableListOf()
)