package mocking.vision.models

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "appointments")
data class Appointments(

        @XmlElement(namespace = "urn:vision", name = "enabled")
        var enabled: Boolean = true

)