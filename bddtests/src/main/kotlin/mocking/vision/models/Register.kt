package mocking.vision.models

import javax.xml.bind.annotation.XmlElement
import javax.xml.bind.annotation.XmlRootElement

@XmlRootElement(name = "authenticationRef")
data class Register(

        @XmlElement(namespace = "urn:vision", name = "rosuAccountId")
        var rosuAccountId: String? = null,
        @XmlElement(namespace = "urn:vision", name = "apiToken")
        var apiToken: String? = null

)

