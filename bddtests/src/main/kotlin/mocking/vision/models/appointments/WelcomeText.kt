package mocking.vision.models.appointments

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "appointments")
data class WelcomeText(

    @XmlElement(namespace = "urn:vision", name = "message")
    var message: String? = null

)