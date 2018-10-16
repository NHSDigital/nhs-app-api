package mocking.vision.models.appointments

import mocking.vision.models.appointments.Slot
import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "slots")
@XmlAccessorType(XmlAccessType.FIELD)
data class Slots(
        @XmlElement(namespace = "urn:vision", name = "slot")
        var slot: List<Slot>? = null
)
