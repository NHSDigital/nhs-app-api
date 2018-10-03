package mocking.vision.models

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "prescriptions")
data class Prescriptions(

        @XmlElement(namespace = "urn:vision", name = "repeat_enabled")
        var repeat_enabled: Boolean = true

)