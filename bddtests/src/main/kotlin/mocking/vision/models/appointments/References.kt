package mocking.vision.models.appointments

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(namespace = "urn:vision")
data class References(
        @XmlElement(namespace = "urn:vision")
        var location: List<Location>? = null,
        @XmlElement(namespace = "urn:vision")
        var owner: List<Owner>? = null,
        @XmlElement(namespace = "urn:vision")
        var session: List<Session>? = null,
        @XmlElement(namespace = "urn:vision")
        var slotType: List<SlotType>? = null
)


