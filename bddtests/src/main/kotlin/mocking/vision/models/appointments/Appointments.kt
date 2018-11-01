package mocking.vision.models.appointments

import mocking.vision.models.appointments.References
import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "appointments")
data class Appointments(

        @XmlElement(namespace = "urn:vision", name = "enabled")
        var enabled: Boolean = true

)
