package mocking.vision.models.appointments

import javax.xml.bind.annotation.XmlAccessType
import javax.xml.bind.annotation.XmlAccessorType
import javax.xml.bind.annotation.XmlAttribute
import javax.xml.bind.annotation.XmlElement

@XmlAccessorType(XmlAccessType.FIELD)
data class Location(
        @XmlAttribute(name = "id") var id: Int?= null,
        @XmlElement(namespace = "urn:vision")
        var name: String
)
