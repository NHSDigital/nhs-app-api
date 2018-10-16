package mocking.vision.models.appointments

import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement

data class Session(
        @XmlAttribute(name = "id") private var id: Int?= null,
        @XmlElement(namespace = "urn:vision")
        var description: String
)
